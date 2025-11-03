using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ItemSorterStudent;

public partial class MainWindow : Window
{
    private readonly ItemSorterRobot robot = new();
    private const int PickDelayMs = 9500;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        robot.Host = "127.0.0.1"; // sæt til din UR/simulator

        OrderBook = new OrderBook();
        Status = "Ready. Click 'Seed demo data' to add two orders.";
        RefreshBindings();
    }

    public OrderBook OrderBook { get; private set; }
    public string Status { get; private set; } = "";

    public void SeedButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var screw = new UnitItem { Name = "M3 screw", PricePerUnit = 1.00m, InventoryLocation = 1, Weight = 0.01m };
        var nut   = new UnitItem { Name = "M3 nut",   PricePerUnit = 1.50m, InventoryLocation = 2, Weight = 0.01m };
        var pen   = new UnitItem { Name = "Pen",      PricePerUnit = 1.00m, InventoryLocation = 3, Weight = 0.02m };

        var a = new Order { Time = DateTime.Now.AddHours(-2) };
        a.OrderLines.Add(new OrderLine { Item = screw, Quantity = 1 });
        a.OrderLines.Add(new OrderLine { Item = nut,   Quantity = 2 });
        a.OrderLines.Add(new OrderLine { Item = pen,   Quantity = 1 });

        var b = new Order { Time = DateTime.Now };
        b.OrderLines.Add(new OrderLine { Item = nut, Quantity = 1 });

        OrderBook.QueueOrder(a);
        OrderBook.QueueOrder(b);

        SetStatus("Seeded 2 orders.");
        RefreshBindings();
    }

    public async void ProcessNextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var lines = OrderBook.ProcessNextOrder();
        if (lines.Count == 0) { SetStatus("No queued orders."); return; }

        SetStatus("Processing order...");
        foreach (var line in lines)
        {
            for (var i = 0; i < (int)line.Quantity; i++)
            {
                SetStatus($"Picking {line.Item.Name} from bin {line.Item.InventoryLocation}...");
                robot.PickUp(line.Item.InventoryLocation);
                await Task.Delay(PickDelayMs);
            }
        }

        SetStatus($"Done. Total revenue: {OrderBook.TotalRevenue():F2}");
        RefreshBindings();
    }

    private void SetStatus(string msg)
    {
        Status = msg;
        LogBox.Text += Environment.NewLine + msg;
    }
    private void RefreshBindings()
    {
        var keep = DataContext; DataContext = null; DataContext = keep;
    }
}

/* ====== Simpelt domæne ====== */
public abstract class Item
{
    public uint InventoryLocation { get; set; }
    public string Name { get; set; } = "";
    public decimal PricePerUnit { get; set; }
}
public sealed class UnitItem : Item { public decimal Weight { get; set; } }
public sealed class BulkItem : Item { public string MeasurementUnit { get; set; } = "kg"; }

public sealed class OrderLine
{
    public Item Item { get; set; } = null!;
    public decimal Quantity { get; set; }
    public override string ToString() => $"{Item.Name} x {Quantity}";
}
public sealed class Order
{
    public DateTime Time { get; set; }
    public ObservableCollection<OrderLine> OrderLines { get; } = new();
    public decimal TotalPrice() { decimal s = 0m; foreach (var l in OrderLines) s += l.Item.PricePerUnit * l.Quantity; return s; }
    public override string ToString() => $"Order {Time:G}: {TotalPrice():F2}";
}
public sealed class OrderBook
{
    public ObservableCollection<Order> QueuedOrders { get; } = new();
    public ObservableCollection<Order> ProcessedOrders { get; } = new();

    public void QueueOrder(Order order) => QueuedOrders.Add(order);

    public System.Collections.Generic.List<OrderLine> ProcessNextOrder()
    {
        var lines = new System.Collections.Generic.List<OrderLine>();
        if (QueuedOrders.Count == 0) return lines;
        var next = QueuedOrders[0];
        QueuedOrders.RemoveAt(0);
        ProcessedOrders.Add(next);
        foreach (var l in next.OrderLines) lines.Add(l);
        return lines;
    }

    public decimal TotalRevenue()
    {
        decimal s = 0m;
        foreach (var o in ProcessedOrders) s += o.TotalPrice();
        return s;
    }
}
