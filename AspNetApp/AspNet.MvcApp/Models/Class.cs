using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetArticle.MvcApp.Models
{
    //public class User
    //{
    //    public int Id { get; set; }
    //    public string? Name { get; set; }
    //    public int Age { get; set; }
    //    public int? CompanyId { get; set; }
    //    public Company? Company { get; set; }
    //}
    //public class Company
    //{
    //    public int Id { get; set; }
    //    public string? Name { get; set; }

    //    public List<User> Users { get; set; } = new();
    //}
    //public class FilterViewModel
    //{
    //    public FilterViewModel(List<Company> companies, int company, string name)
    //    {
    //        // устанавливаем начальный элемент, который позволит выбрать всех
    //        companies.Insert(0, new Company { Name = "Все", Id = 0 });
    //        Companies = new SelectList(companies, "Id", "Name", company);
    //        SelectedCompany = company;
    //        SelectedName = name;
    //    }
    //    public SelectList Companies { get; } // список компаний
    //    public int SelectedCompany { get; } // выбранная компания
    //    public string SelectedName { get; } // введенное имя
    //}

    //public class ArticlesCategoryViewModel1
    //{
    //    public List<ArticleModel>? Articles { get; set; } 
    //    public SelectList? Categories { get; set; }
    //    public string? SelectedCategory { get; set; } = "Все";
    //    public string? SearchString { get; set; }
    //    public PageViewModel PageViewModel { get; set; } 
    //}
    //public class IndexViewModel
    //{
    //    public IEnumerable<User> Users { get; }
    //    public PageViewModel PageViewModel { get; }
    //    public FilterViewModel FilterViewModel { get; }


    //    //public IndexViewModel(IEnumerable<User> users, PageViewModel pageViewModel,
    //    //    FilterViewModel filterViewModel, SortViewModel sortViewModel)
    //    //{
    //    //    Users = users;
    //    //    PageViewModel = pageViewModel;
    //    //    FilterViewModel = filterViewModel;
    //    //    SortViewModel = sortViewModel;
    //    //}
    //}
   
}
