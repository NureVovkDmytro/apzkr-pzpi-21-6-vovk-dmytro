using System.Text.Json;

namespace Discerniy.Domain.Entity.SubEntity
{
    public class DeviceCommand<T> where T : class
    {
        public string Command { get; set; }
        public T? Payload { get; set; } = default;

        public DeviceCommand(string command, T? payload = default)
        {
            Command = command;
            Payload = payload;
        }

        public DeviceCommand(string command)
        {
            Command = command;
        }

        public DeviceCommand()
        {
            Command = string.Empty;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
