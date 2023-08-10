using System.ComponentModel.DataAnnotations;

namespace UseCase3._0.Models
{
    public class Model
    {
        [Required(ErrorMessage = " Choose an option.")]
        public string Option { get; set; }

        [Required(ErrorMessage = " Enter a prompt.")]
        public string PromptText { get; set; }

        public string GeneratedResponse { get; set; }

        public string Email { get; set; }
    }

}

