using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Repositories.Model;

public class t_Feedback
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? c_fid{get;set;}

    public string c_role{get;set;}

    public string c_user_email{get;set;}

    public int c_rating{get;set;}

    public string c_feedback_msg{get;set;}

    public string? c_date{get;set;}
}