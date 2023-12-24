namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; } = false; // default value
        public DateTime? DateRead { get; set; } // ? means nullable
        public DateTime MessageSent { get; set; } = DateTime.UtcNow; // default value
        public DateTime? DateDeleted { get; set; }
        public bool SenderDeleted { get; set; } = false;
        public bool RecipientDeleted { get; set; } = false;
        
        
    }
}