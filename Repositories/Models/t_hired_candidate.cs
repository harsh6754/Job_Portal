using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Model;

public class t_hired_candidate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_hire_id{get;set;}

    public int c_user_id{get;set;}
    public t_user? user{get;set;}

    public int c_company_id{get;set;}

    public string c_status{get;set;}
    public int? c_job_id{get;set;}
    public Job_Post? job_Post{get;set;}

    public string? c_hired_date{get;set;}

    [NotMapped]
    public string? c_company_name{get;set;}
    [NotMapped]
    public string? c_company_logo{get;set;}
    [NotMapped]
    public string? c_company_email{get;set;}
    [NotMapped]
    public string? c_user_email{get;set;}
    [NotMapped]
    public string? c_fullName{get;set;}
}