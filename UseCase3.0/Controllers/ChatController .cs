using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace UseCase3._0.Controllers
{
    public class ChatController : Controller
    {
        const string API_KEY = "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";
        private readonly ILogger<ChatController> _logger;
        static readonly HttpClient client = new HttpClient();

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(string option, string prompt)
        {
            ViewBag.UserOption = option;
            ViewBag.Prompt = prompt;

            var options = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = option
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                max_tokens = 3500,
                temperature = 0.2
            };

            var json = JsonConvert.SerializeObject(options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", API_KEY);

            try
            {
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                string result = jsonResponse.choices[0].message.content;

                ViewBag.Response = result; // Store the response in ViewBag

                return View("Get"); // Return the Get view to display the generated response
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string response, string email)
        {
            try
            {
                // Encode the response to HTML entities
                var encodedResponse = WebUtility.HtmlEncode(response);

                // Create a new email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Ashish", "ashishvaishya1099@gmail.com")); //  your email
                message.To.Add(new MailboxAddress("Recipient", email)); // Use MailboxAddress constructor with display name and email address
                message.Subject = "Generated Response from ChatBot";

                // Create the plain-text version of the message body
                var plainTextBody = response;

                // Create the HTML version of the message body
                var htmlBody = $"<pre>{encodedResponse}</pre>";

                // Create the plain-text and HTML versions of the message
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = plainTextBody;
                bodyBuilder.HtmlBody = htmlBody;

                message.Body = bodyBuilder.ToMessageBody();

                // Send the email using MailKit with your SMTP settings
                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, false); // Replace with your SMTP server details
                await client.AuthenticateAsync("ashishvaishya1099@gmail.com", "pborjoxdavbumpmu"); // Replace with your email and password
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Handle email sending errors here
                return Json(ex.Message);
            }

            ViewBag.EmailSent = true;
            return View("Get");
        }

    }
}









//using Microsoft.AspNetCore.DataProtection.KeyManagement;
//using Microsoft.AspNetCore.Mvc;
//using OpenAI;
//using System;
//using System.Collections.Generic;
//using System.Net.Mail;

//namespace UseCase3._0.Controllers
//{
//    public class ChatController : Controller
//    {
//        const string apiKey = "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"; // Replace with your OpenAI API key
//        const string engine = "text-codex-003"; // You can also try "text-codex" if available
//        const string prompt = "You: ";

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Index(string message)
//        {
//            var chatbotResponse = GetChatbotResponse(message);

//            ViewData["ChatResponse"] = chatbotResponse;
//            return View();
//        }

//        private string GetChatbotResponse(string userMessage)
//        {
//            OpenAI.ApiKey = apiKey;

//            switch (userMessage.ToLower())
//            {
//                case "i":
//                case "need answer":
//                    return GetChatbotResponseWithPrompt("Please ask your question: ");
//                case "ii":
//                case "need code":
//                    return GetChatbotResponseWithPrompt("Please provide your code prompt: ");
//                case "iii":
//                case "need suggestion":
//                    return GetChatbotResponseWithPrompt("Please enter your suggestion prompt: ");
//                default:
//                    return "Chatbot: I'm sorry, I couldn't understand your choice. Please select a valid option.";
//            }
//        }

//        private string GetChatbotResponseWithPrompt(string promptMessage)
//        {
//            Console.Write(promptMessage);
//            string userMessage = Console.ReadLine();
//            var response = OpenAI.Engine(engine).Complete(prompt + userMessage);

//            if (response != null && response.Choices.Count > 0)
//            {
//                string chatbotResponse = response.Choices[0].Text.Trim();
//                return "Chatbot: " + chatbotResponse;
//            }
//            else
//            {
//                return "Chatbot: I'm sorry, I couldn't generate a response.";
//            }
//        }
//    }
//}



