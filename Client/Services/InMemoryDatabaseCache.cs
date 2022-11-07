﻿using Shared.Models;
using System.Linq;

namespace Client.Services;

internal sealed class InMemoryDatabaseCache
{
	private readonly HttpClient _httpClient;

	public InMemoryDatabaseCache(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

    #region Categories
    private List<Category> _categories = null;

    internal List<Category> Categories
    {
        get => _categories;
        set
        {
            _categories = value;
            NotifyCategoriesDataChanged();
        }
    }

    internal async Task<Category> GetCategoryByCategoryId(int categoryId, bool withPosts)
    {
        if (_categories == null)
        {
            await GetCategoriesFromDatabaseAndCache(withPosts);
        }

        Category categoryToReturn = _categories?.First(c => c.CategoryId == categoryId);

        if (categoryToReturn?.Posts is null && withPosts)
        {
            categoryToReturn = await _httpClient.GetFromJsonAsync<Category>($"{APIEndpoints.s_categoriesWithPosts}/{categoryToReturn.CategoryId}");
        }

        return categoryToReturn!;
    }

    internal async Task<Category> GetCategoryByCategoryName(string categoryName, bool withPosts, bool nameToLowerFromUrl)
    {
        if (_categories is null)
        {
            await GetCategoriesFromDatabaseAndCache(withPosts);
        }

        Category categoryToReturn = null;

        if (nameToLowerFromUrl)
        {
            categoryToReturn = _categories.First(c => c.Name.ToLowerInvariant() == categoryName);
        }
        else
        {
            categoryToReturn = _categories.First(c => c.Name == categoryName);
        }

        if (categoryToReturn.Posts is null && withPosts)
        {
            categoryToReturn = await _httpClient.GetFromJsonAsync<Category>($"{APIEndpoints.s_categoriesWithPosts}/{categoryToReturn.CategoryId}");
        }

        return categoryToReturn;
    }

    private bool GettingCategoriesFromDatabaseAndCaching = false;

    internal async Task GetCategoriesFromDatabaseAndCache(bool withPosts)
    {
        // Only allow Get request to run at a time
        if (!GettingCategoriesFromDatabaseAndCaching)
        {
            GettingCategoriesFromDatabaseAndCaching = true;

            List<Category> categoriesFromDatabase = null;

            if (_categories != null)
            {
                _categories = null;
            }

            if (withPosts)
            {
                categoriesFromDatabase = await _httpClient.GetFromJsonAsync<List<Category>>(APIEndpoints.s_categoriesWithPosts);
            }
            else
            {
                categoriesFromDatabase = await _httpClient.GetFromJsonAsync<List<Category>>(APIEndpoints.s_categories);
            }

            _categories = categoriesFromDatabase?.OrderByDescending(c => c.CategoryId).ToList();

            if (withPosts)
            {
                List<Post> postsFromCategories = new();

                foreach (var category in _categories!)
                {
                    if (category.Posts.Count != 0)
                    {
                        foreach (var post in category.Posts)
                        {
                            Category postCategoryWithoutPosts = new Category
                            {
                                CategoryId = category.CategoryId,
                                ThumbnailImagePath = category.ThumbnailImagePath,
                                Name = category.Name,
                                Description = category.Description,
                                Posts = null
                            };

                            post.Category = postCategoryWithoutPosts;

                            postsFromCategories.Add(post);
                        }
                    }
                }

                _posts = postsFromCategories.OrderByDescending(post => post.PostId).ToList();
            }

            GettingCategoriesFromDatabaseAndCaching = false;
        }
    }

    internal event Action OnCategoriesDataChanged;

    private void NotifyCategoriesDataChanged() => OnCategoriesDataChanged?.Invoke();
    #endregion

    #region Posts
    private List<Post> _posts = null;

    internal List<Post> Posts
    {
        get => _posts;
        set
        {
            _posts = value;
            NotifyPostsDataChanged();
        }
    }

    internal async Task<Post> GetPostByPostId(int postId)
    {
        if (_posts is null)
        {
            await GetPostsFromDatabaseAndCache();
        }

        return _posts.First(post => post.PostId == postId);
    }

    internal async Task<PostDTO> GetPostDTOByPostId(int postId) => await _httpClient.GetFromJsonAsync<PostDTO>($"{APIEndpoints.s_postsDTO}/{postId}");

    private bool _gettingPostsFromDatabaseAndCaching = false;
    internal async Task GetPostsFromDatabaseAndCache()
    {
        // Only allow one get to run at a time
        if (!_gettingPostsFromDatabaseAndCaching)
        {
            _gettingPostsFromDatabaseAndCaching = true;

            if (_posts is not null)
            {
                _posts = null;
            }

            List<Post> postsFromDatabase = await _httpClient.GetFromJsonAsync<List<Post>>(APIEndpoints.s_posts);

            _posts = postsFromDatabase.OrderByDescending(post => post.PostId).ToList();

            _gettingPostsFromDatabaseAndCaching = false;
        }
    }

    internal event Action OnPostsDataChanged;
    private void NotifyPostsDataChanged() => OnPostsDataChanged?.Invoke();

    #endregion
}
