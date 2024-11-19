﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockManagement_WinUI3;

public class ProductsViewModel
{
    public ObservableCollection<ProductsItem> Items { get; set; } = [];
    public ObservableCollection<string> StatusOptions = [
        "Active",
        "Inactive"
    ];
    public bool IsInitialLoadComplete { get; set; } = false;

    public static async Task LoadProducts(ProductsViewModel viewModel)
    {
        try
        {
            viewModel.Items.Clear();
            viewModel.IsInitialLoadComplete = false;

            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var response = await httpClient.GetAsync(new Uri("http://localhost:5177/api/products"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch products");
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("Response content is empty");
            }

            var products = JsonConvert.DeserializeObject<List<Product>>(content);

            if (products != null)
            {
                foreach (var product in products)
                {
                    viewModel.Items.Add(new ProductsItem
                    {
                        Code = product.id.ToString(),
                        Name = product.name,
                        Status = product.status ? "Active" : "Inactive"
                    });
                }
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

    public static async Task AddProduct(ProductsViewModel viewModel, string name, bool status)
    {
        try
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(new { name, status }), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(new Uri("http://localhost:5177/api/products"), content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to add product");
            }

            await LoadProducts(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static async Task DeleteProduct(ProductsViewModel viewModel, int id)
    {
        try
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var response = await httpClient.DeleteAsync(new Uri($"http://localhost:5177/api/products/{id}"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to delete product {id}");
            }

            await LoadProducts(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static async Task UpdateProduct(Product product)
    {
        try
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "jn4cfes9e30yc7roymve79yu.bdhfs5xc13z30ri1yruoi5u2");
            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(product), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PatchAsync(new Uri($"http://localhost:5177/api/products/{product.id}"), content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update product {product.id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}


public class ProductsItem : INotifyPropertyChanged
{
    private string _code;
    private string _name;
    private string _status;

    public string Code
    {
        get => _code;
        set
        {
            _code = value;
            OnPropertyChanged(nameof(Code));
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            var noName = string.IsNullOrEmpty(_name);
            _name = value;
            OnPropertyChanged(nameof(Name));

            if (noName) return;

            var product = new Product
            {
                id = int.Parse(_code),
                name = _name,
                status = _status == "Active"
            };

            _ = ProductsViewModel.UpdateProduct(product);
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

            if (noStatus) return;

            var product = new Product
            {
                id = int.Parse(_code),
                name = _name,
                status = _status == "Active"
            };

            _ = ProductsViewModel.UpdateProduct(product);
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}