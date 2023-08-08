// See https://aka.ms/new-console-template for more information

using AngleSharp;

Console.WriteLine("Hello, World!");

using var httpClient = new HttpClient { BaseAddress = new Uri("https://abit.itmo.ru/") }; //RatingPage_table__FbzTn
await using var htmlStream = await httpClient.GetStreamAsync("rating/master/budget/7488");

var configuration = Configuration.Default;
using var context = new BrowsingContext(configuration);
using var document = await context.OpenAsync(response => response.Content(htmlStream));

var participants = document.QuerySelectorAll(".RatingPage_table__item__qMY0F")
    .Select(element =>
    {
        var priorityTrialTypeBlock = element.QuerySelectorAll(".RatingPage_table__infoLeft__Y_9cA>p>span");

        var id = element.QuerySelector(".RatingPage_table__position__uYWvi>span")!.TextContent;
        var priority = int.Parse(priorityTrialTypeBlock.First().TextContent);
        var allPoints = double.Parse(element
            .QuerySelectorAll(".RatingPage_table__infoLeft__Y_9cA>p")
            .Single(subElement => subElement.TextContent.Contains("Балл ВИ+ИД: ")).TextContent
            .Split("Балл ВИ+ИД: ")
            .Last());
        var trialType = priorityTrialTypeBlock.Skip(1).First().TextContent;
        var isOriginalDocument = element
                .QuerySelectorAll(".RatingPage_table__info__quwhV>div")
                .Single(subElement => subElement.QuerySelector("p")!.TextContent.Contains("Оригиналы документов: "))
                .QuerySelector("span")?.TextContent switch
                {
                    "да" => true,
                    "нет" => false,
                    _ => throw new NotImplementedException()
                };

        var participant = new Participant
        {
            Id = id,
            Priority = priority,
            AllPoints = allPoints,
            TrialType = trialType,
            IsOriginalDocument = isOriginalDocument
        };

        return participant;
    })
    .ToArray();

var temp = participants
    .Where(participant => participant is { AllPoints: >= 100, IsOriginalDocument: true })
    .ToArray();

var count = temp.Length;

Console.WriteLine();

public record Participant
{
    public required string Id { get; init; }

    public int Priority { get; init; }

    public double AllPoints { get; init; }

    public required string TrialType { get; init; }

    public bool IsOriginalDocument { get; init; }
}


