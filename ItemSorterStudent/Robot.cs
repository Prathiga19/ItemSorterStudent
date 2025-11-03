using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace ItemSorterStudent;

public class Robot
{
    public string Host { get; set; } = "127.0.0.1";
    public int DashboardPort { get; set; } = 29999; // brake release
    public int UrscriptPort  { get; set; } = 30002; // URScript

    protected void SendRaw(int port, string message)
    {
        using var client = new TcpClient();
        client.Connect(Host, port);
        var data = Encoding.ASCII.GetBytes(message);
        using var stream = client.GetStream();
        stream.Write(data, 0, data.Length);
    }

    public void SendUrscript(string urscript)
    {
        SendRaw(DashboardPort, "brake release\n");
        SendRaw(UrscriptPort, urscript);
    }
}

public class ItemSorterRobot : Robot
{
    private const string Template = @"
def move_item_to_shipment_box():
    SBOX_X = 3
    SBOX_Y = 3
    ITEM_X = {0}
    ITEM_Y = 1
    DOWN_Z = 1

    def moveto(x, y, z = 0):
        SEP = 0.1
        p = p[0 + x * SEP, -.4 + y * SEP, .2 + z * SEP, d2r(180), 0, 0]
        movej(get_inverse_kin(p))
    end

    moveto(ITEM_X, ITEM_Y)
    moveto(ITEM_X, ITEM_Y, -DOWN_Z)  # pick
    moveto(ITEM_X, ITEM_Y)
    moveto(SBOX_X, SBOX_Y)           # drop
end
";
    public void PickUp(uint binIndex)
    {
        var urs = string.Format(CultureInfo.InvariantCulture, Template, binIndex);
        SendUrscript(urs);
    }
}