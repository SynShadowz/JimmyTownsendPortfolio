using System.Linq;

namespace Client.Services;

internal sealed class InMemoryDatabaseCache
{
	private readonly HttpClient _httpClient;

	public InMemoryDatabaseCache(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	private List<Category>? _categories = null;

	internal List<Category>? Categories
	{
		get => _categories;
		set
		{
			_categories = value;
			NotifyCategoriesDataChanged();
		}
	}

	internal async Task<Category> GetCategoryByCategoryId(int categoryId)
	{
		if (_categories == null)
		{
			await GetCategoriesFromDatabaseAndCache();
		}

		return _categories.First(c => c.CategoryId == categoryId);
	}


    private bool GettingCategoriesFromDatabaseAndCaching = false;

    internal async Task GetCategoriesFromDatabaseAndCache()
	{
		// Only allow Get request to run at a time
		if (!GettingCategoriesFromDatabaseAndCaching)
		{
			GettingCategoriesFromDatabaseAndCaching = true;
            _categories = await _httpClient.GetFromJsonAsync<List<Category>>(APIEndpoints.s_categories);
			GettingCategoriesFromDatabaseAndCaching = false;
        }
    }

    internal event Action OnCategoriesDataChanged;

	private void NotifyCategoriesDataChanged() => OnCategoriesDataChanged?.Invoke();
}
