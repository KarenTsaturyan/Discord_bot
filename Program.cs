using DiscordBot_Ecommerce;
using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Transcendental ECommerce Discord Online Bot Advanced Console app, or TECDOBACA in short");
        Console.WriteLine("To start the bot press any key");
        Console.ReadKey();

        DiscordBot bot = new DiscordBot("TOKEN");
        DiscordChannel channel = new DiscordChannel(bot, channelId: "Cannel ID");
        DirectMessage directMessage = new DirectMessage(bot);

        string json = File.ReadAllText("products.json");
        Dictionary<string, string> products = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        Task task = MessageProcessor(channel, products, directMessage);
        task.Wait();

        Console.WriteLine("Process finalized");
        Console.WriteLine("Shutting down...");
    }

    public async static Task MessageProcessor(DiscordChannel channel, Dictionary<string, string> products, DirectMessage directMessage)
    {
        // send product list
        await channel.SendMessage(MessageTemplates.ProductList(products));
        Console.WriteLine("The message containing the product list has been sent out");
        Console.WriteLine("Messages are read each 5 seconds.To exit the bot, press Esc.");

        while (true)
        {
            Console.WriteLine("Waiting... Press Esc to quit.");
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }

            await Task.Delay(5 * 1000);// 5 second
            IEnumerable<UserMessage> messagesInTheChannel = await
            channel.ReadMessages();

            Console.WriteLine("Messages read from the channel");
            foreach (var message in messagesInTheChannel)
            {
                if (products.TryGetValue(message.Content, out string? price))// here we match key and retriev value(Price)
                {
                    Console.WriteLine($"\nFound a price query, $the response from the bot is: ${message.Content} costs {price} ");
                    string priceReport = MessageTemplates.PriceReport(message.Content, price);
                    await directMessage.Send(message.UserId, priceReport);
                    // Delete message after we ansered directly
                    await channel.DeleteMessage(message.MessageId);
                }
            }
            Console.WriteLine("\nNo more queries have been found");
        }
    }
}