// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.0

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace SantaBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly HttpClient _httpClient;
        private readonly ImageAnalyzer _imageAnalyzer;

        public EchoBot(ImageAnalyzer imageAnalyzer)
        {
            _imageAnalyzer = imageAnalyzer;
            _httpClient = new HttpClient();
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //var replyText = $"Echo: {turnContext.Activity.Text}";
            //await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            var result = await DoSomething(turnContext);
            await turnContext.SendActivityAsync(result, cancellationToken: cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Ho Ho Ho, Santa and the elves are here to help you!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        private async Task<string> DoSomething(ITurnContext<IMessageActivity> turnContext)
        {
            var result = new ImageAnalysis();
            var stringResponse = "";
            if (turnContext.Activity.Attachments?.Count > 0)
{
                var attachment = turnContext.Activity.Attachments[0];
                var image = await _httpClient.GetStreamAsync(attachment.ContentUrl);
                if (image != null)
                {
                    result = await _imageAnalyzer.AnalyzeImageAsync(image);
                    
                stringResponse = $"So Santa thinks the present you got is \"{result.Description.Captions[0].Text.ToUpperInvariant()}\"\r\nIs it broken and do you want to find a return location close by?";
                }
            }
            else
            {
                if (turnContext.Activity.Text == "yes")
                {
                    _httpClient.BaseAddress = new System.Uri("https://getsanta.azurewebsites.net");
                    var json = JsonSerializer.Deserialize<SantaLocation>(await _httpClient.GetStringAsync("/api/getsantalocation"));
                    stringResponse = $"Elve 18 says: Santa will be with you after delivering {json.presents} presents\r\n His current location is {json.address.municipality}";

                }
                else
                {
                    stringResponse = $"Santa just repeats: {turnContext.Activity.Text}";
                }
            }

            
            return stringResponse;
        }
    }
}
