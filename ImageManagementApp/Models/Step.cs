using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagementApp.Models
{
    public class Step
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public StepStatus Status { get; set; }

        public Step()
        {
            Id = Guid.NewGuid().ToString();
            Name = $"Step {DateTime.Now:HHmmss}";
            ImagePath = "";
            Status = StepStatus.Active;
        }

        public Step Copy()
        {
            return new Step
            {
                Id = this.Id,
                Name = this.Name,
                ImagePath = this.ImagePath,
                Status = this.Status
            };
        }
    }

    public enum StepStatus
    {
        Active,
        PendingDeletion
    }

}
