using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entitites
{
    public class KafkaMessage
    {
        [Key]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public EKafkaMessageStatus Status { get; set; }
        public long Offset { get; set; }
        public int Partition { get; set; }
        public DateTime Timestamp { get; set; }
        public string MessageData { get; set; }

        public void CopyData(KafkaMessage copyData)
        {
            this.MessageId = MessageId;
            this.Status = Status;
            this.Offset = Offset;
            this.Partition = Partition;
            this.Timestamp = Timestamp;
            this.MessageData = MessageData;
        } 
    }
}
