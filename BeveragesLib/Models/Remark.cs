using Microsoft.EntityFrameworkCore;

namespace BeveragesLib.Models;

[Index(nameof(Name), IsUnique = true)]
public class Remark: IdName
{
    public int Order { get; set; }

    public static List<Remark> Seed()
    {
        return
            [
                new Remark { Name = "Hemskt", Order = 1 },
                new Remark { Name = "Inte bra", Order = 2 },
                new Remark { Name = "Medelmåttligt", Order = 3 },
                new Remark { Name = "Bra", Order = 4 },
                new Remark { Name = "Strålande", Order = 5 }
            ];
    }
}
