using Libro.Presentation.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.BookReview
{
    public class GetBookReviewWithUserDto:GetBookReviewDto
    {
        public UserWithIdAndNameDto User { get; set; }
    }
}
