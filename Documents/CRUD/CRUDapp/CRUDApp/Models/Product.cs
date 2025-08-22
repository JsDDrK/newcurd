using System.ComponentModel.DataAnnotations;

namespace CRUDApp.Models;

public class Product
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "작성자는 필수 입력 항목입니다.")]
    public required string Name { get; set; } // 작성자 이름
    
    [Required(ErrorMessage = "제목은 필수 입력 항목입니다.")]
    [StringLength(100, ErrorMessage = "제목은 100자 이내로 작성해야 합니다.")] // 제목 필드 추가
    public required string Title { get; set; } 

    [Required(ErrorMessage = "내용은 필수 입력 항목입니다.")]
    [StringLength(1000, ErrorMessage = "내용은 1000자 이내로 작성해야 합니다.")]
    public required string Description { get; set; } // 게시물 내용

    [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}