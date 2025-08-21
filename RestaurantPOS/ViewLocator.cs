using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using RestaurantPOS.ViewModels;

namespace RestaurantPOS;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null) return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View");
        if (!name.Contains(".Views."))
            name = name.Replace("RestaurantPOS.", "RestaurantPOS.Views.");

        // ДЭЭР НЬ DEBUG хэвлэл нэмнэ
         //Console.WriteLine($"[ViewLocator] Looking for view: {name}");

        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType(name))
            .FirstOrDefault(t => t != null);

        if (type != null)
        {
            // Console.WriteLine($"[ViewLocator] Found: {type.FullName}");
            return (Control)Activator.CreateInstance(type)!;
        }

        //Console.WriteLine($"[ViewLocator] View not found for: {name}");
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
