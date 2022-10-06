using System.ComponentModel.DataAnnotations; //Grants access to annotations for validation

namespace ProjName.Models
{
    public class ContactViewModel
    {
        //We can use Data Annotations to add validation to our model.
        //This is useful when we have required fields or need certain types of info.
        [Required(ErrorMessage = "*Name is required")]  //Makes the field required
        public string Name { get; set; }

        [Required(ErrorMessage = "*Email is required")]
        [DataType(DataType.EmailAddress)]  //Certain formatting is expected (@ symbol, .com. etc.)
        public string Email { get; set; }

        [Required(ErrorMessage = "*Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "*Message is required")]
        [DataType(DataType.MultilineText)]  //Makes the textbox for this field bigger
        public string Message { get; set; }
    }

    //MINI-LAB
    //In your personalsite MVC solution, create the ContactViewModel.
    //You can copy & paste the above propertied and annotaions.
    //Dont forget to add a using statement for the annotations!
}
