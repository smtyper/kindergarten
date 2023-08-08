// See https://aka.ms/new-console-template for more information

using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

using var httpClient = new HttpClient { BaseAddress = new Uri("https://abiturient.kpfu.ru/") };
await using var htmlStream = await httpClient.GetStreamAsync("/entrant/abit_entrant_originals_list?p_inst=0&p_faculty=47&p_speciality=767&p_typeofstudy=1&p_category=1");
using var streamReader = new StreamReader(htmlStream, Encoding.GetEncoding(1251));

var htmlString = await streamReader.ReadToEndAsync();

var configuration = Configuration.Default;
using var context = new BrowsingContext(configuration);
using var document = await context.OpenAsync(response => response.Content(htmlString));

var participants = document
    .QuerySelectorAll<IHtmlTableRowElement>("#t_common > tbody > tr")
    .Skip(2)
    .Select(rowElement =>
    {
        var numberInListString = rowElement.Cells[0].TextContent.Trim();
        var idString = rowElement.Cells[1].TextContent.Trim();
        var scoreString = rowElement.Cells[2].TextContent.Trim();
        var isPreemptiveRightString = rowElement.Cells[3].TextContent.Trim();
        var priorityString = rowElement.Cells[4].TextContent.Trim();
        var isOriginalDocumentString = rowElement.Cells[5].TextContent.Trim();
        var statusString = rowElement.Cells[6].TextContent.Trim();
        var commentString = rowElement.Cells[7].TextContent.Trim();

        var participant = new Participant
        {
            NumberInList = int.Parse(numberInListString),
            Id = idString,
            Score = string.IsNullOrEmpty(scoreString) ?
                null :
                decimal.Parse(scoreString),
            IsPreemptiveRight = isPreemptiveRightString switch
            {
                "да" => true,
                "нет" => false,
                _ => throw new ArgumentOutOfRangeException(nameof(isPreemptiveRightString))
            },
            Priority = int.Parse(priorityString),
            IsOriginalDocument = isOriginalDocumentString switch
            {
                "да" => true,
                "нет" => false,
                _ => throw new ArgumentOutOfRangeException(nameof(isPreemptiveRightString))
            },
            Status = statusString,
            Comment = commentString
        };

        return participant;
    })
    .ToArray();

var userParticipant = participants.Single(participant => participant.Id is "163-262-496-63");

var isOriginalPriorityRaiting = participants
    .Where(participant => participant is { IsOriginalDocument: true, Priority: 1 })
    .Append(userParticipant with { IsOriginalDocument = true, Priority = 1 })
    .Distinct()
    .OrderByDescending(participant => participant.Score)
    .Select((participant, index) => (place: index + 1, participant))
    .ToArray();
var opUserPlace = isOriginalPriorityRaiting.Single(p => p.participant.Id == userParticipant.Id);

var isOriginalRaiting = participants
    .Where(participant => participant is { IsOriginalDocument: true })
    .Append(userParticipant with { IsOriginalDocument = true })
    .Distinct()
    .OrderByDescending(participant => participant.Score)
    .Select((participant, index) => (place: index + 1, participant))
    .ToArray();
var oUserPlace = isOriginalRaiting.Single(p => p.participant.Id == userParticipant.Id);

var competitors = participants
    .Where(participant => participant.Id != userParticipant.Id &&
                                                                   participant.Score >= userParticipant.Score &&
                                                                   !participant.IsOriginalDocument)
    .ToArray();

Console.WriteLine($"Your place in original + priority raiting is: {opUserPlace.place}.");
Console.WriteLine($"Your place in original is: {oUserPlace.place}.");
Console.WriteLine($"Сompetitors count: {competitors.Length}");

public record Participant
{
    public int NumberInList { get; init; }

    public required string Id { get; init; }

    public decimal? Score { get; init; }

    public bool IsPreemptiveRight { get; init; }

    public int Priority { get; init; }

    public bool IsOriginalDocument { get; init; }

    public required string Status { get; init; }

    public string? Comment { get; init; }
}



