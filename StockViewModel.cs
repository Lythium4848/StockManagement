using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockManagement_WinUI3;

public class StockViewModel
{
    public ObservableCollection<StockItem> Items { get; set; } = [];

    public readonly ObservableCollection<string> StatusOptions =
    [
        "Active",
        "Inactive"
    ];

    public bool IsInitialLoadComplete { get; set; } = false;

    private static async Task<List<Stock>> GetStock()
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var response = await httpClient.GetAsync(new Uri("http://localhost:5177/api/stock"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch stock");
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("Response content is empty");
            }

            var stockList = JsonConvert.DeserializeObject<List<Stock>>(content);

            return stockList ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return [];
        }
    }

    public static async Task LoadStock(StockViewModel viewModel)
    {
        try
        {
            viewModel.Items.Clear();
            viewModel.IsInitialLoadComplete = false;

            var stockList = await GetStock();

            foreach (var stock in stockList)
            {
                viewModel.Items.Add(new StockItem
                {
                    Id = stock.Id,
                    Name = stock.Name,
                    TransactionDate = stock.TransactionDate,
                    Quantity = stock.Quantity,
                    Status = stock.Status ? "Active" : "Inactive",
                    IsInitialLoadComplete = true
                });
            }

            viewModel.IsInitialLoadComplete = true;
        }
        catch (Exception ex)
        {
            viewModel.Items.Clear();
            Console.WriteLine($"An error occurred: {ex.Message}");
            viewModel.IsInitialLoadComplete = true;
        }
    }

    public static async Task AddStock(StockViewModel viewModel, string name, DateTime transactionDate, int quantity,
        bool status)
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var content =
                new StringContent(JsonConvert.SerializeObject(new { name, status, transactionDate, quantity }),
                    Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(new Uri("http://localhost:5177/api/stock"), content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to add stock");
            }

            await LoadStock(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static async Task DeleteStock(StockViewModel viewModel, int id)
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var response = await httpClient.DeleteAsync(new Uri($"http://localhost:5177/api/stock/{id}"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to delete stock {id}");
            }

            await LoadStock(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static async Task UpdateStock(Stock stock)
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var content = new StringContent(JsonConvert.SerializeObject(stock), Encoding.UTF8, "application/json");
            var response = await httpClient.PatchAsync(new Uri($"http://localhost:5177/api/stock/{stock.Id}"), content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update stock {stock.Id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

public class StockItem : INotifyPropertyChanged
{
    private readonly int _id = 0;
    private readonly string _name = "";
    private DateTime _transactionDate = DateTime.UtcNow;
    private int _quantity = 0;
    private string _status = "Active";
    public bool IsInitialLoadComplete { get; init; } = false; //adw

    public int Id
    {
        get => _id;
        init
        {
            _id = value;
            OnPropertyChanged(nameof(Id));
        }
    }

    public string Name
    {
        get => _name;
        init
        {
            var noName = string.IsNullOrEmpty(_name);
            _name = value;
            OnPropertyChanged(nameof(Name));

            if (noName || !IsInitialLoadComplete) return;

            var stock = new Stock
            {
                Id = _id,
                Name = _name,
                TransactionDate = _transactionDate,
                Quantity = _quantity,
                Status = _status == "Active"
            };

            _ = StockViewModel.UpdateStock(stock);
        }
    }

    public DateTime TransactionDate
    {
        get => _transactionDate;
        set
        {
            _transactionDate = value;
            OnPropertyChanged(nameof(TransactionDate));

            if (!IsInitialLoadComplete) return;

            var stock = new Stock
            {
                Id = _id,
                Name = _name,
                TransactionDate = _transactionDate,
                Quantity = _quantity,
                Status = _status == "Active"
            };

            _ = StockViewModel.UpdateStock(stock);
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged(nameof(Quantity));

            if (!IsInitialLoadComplete) return;

            var stock = new Stock
            {
                Id = _id,
                Name = _name,
                TransactionDate = _transactionDate,
                Quantity = _quantity,
                Status = _status == "Active"
            };

            _ = StockViewModel.UpdateStock(stock);
        }
    }

    public string Status
    {
        get => _status;
        set
        {
            var noStatus = string.IsNullOrEmpty(_status);
            _status = value;
            OnPropertyChanged(nameof(Status));

            if (noStatus || !IsInitialLoadComplete) return;

            var stock = new Stock
            {
                Id = _id,
                Name = _name ?? "",
                TransactionDate = _transactionDate,
                Quantity = _quantity,
                Status = _status == "Active"
            };

            _ = StockViewModel.UpdateStock(stock);
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}