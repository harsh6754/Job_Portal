using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class ChatMessage
    {
        public string UserMessage { get; set; } 
        public string BotResponse { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsUserMessage { get; set; } = true; // true for user message, false for bot response
        public string ImageUrl { get; set; } // URL for the image if any    
        
    }
}