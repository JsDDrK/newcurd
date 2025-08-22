using CRUDApp.Data;
using CRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Session 사용을 위해 추가

namespace CRUDApp.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private const string AdminId = "admin"; 
    private const string AdminPassword = "JsDDr!408"; 

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Products (모든 게시물 목록 표시)
    public async Task<IActionResult> Index()
    {
        ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true"; 
        return View(await _context.Products.ToListAsync());
    }

    // GET: Products/Details/5 (게시물 내용 팝업창)
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }


    // GET: Products/Create (게시물 생성 페이지 표시)
    public IActionResult Create()
    {
        ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true"; 
        return View();
    }

    // POST: Products/Create (새 게시물 생성 처리)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Title,Description,Password")] Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Edit/5 (특정 게시물 수정 페이지 표시)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true"; // 수정 페이지에도 관리자 여부 전달
        return View(product);
    }

    // POST: Products/Edit/5 (게시물 수정 처리)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Title,Description,Password")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        var originalProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (originalProduct == null || originalProduct.Password != product.Password)
        {
            ModelState.AddModelError("Password", "비밀번호가 올바르지 않습니다."); 
            ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true"; // 오류 발생 시 관리자 여부 다시 전달
            return View(product); 
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw; 
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true"; // 유효성 검사 실패 시 관리자 여부 다시 전달
        return View(product);
    }

    // GET: Products/Delete/5 (특정 게시물 삭제 확인 페이지 표시)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true"; // 삭제 페이지에도 관리자 여부 전달
        return View(product);
    }

    // POST: Products/Delete/5 (게시물 삭제 처리)
    [HttpPost, ActionName("Delete")] 
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, [Bind("Password")] Product product) 
    {
        bool isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

        // 관리자가 아닐 때만 비밀번호 확인
        if (!isAdmin)
        {
            var originalProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (originalProduct == null || originalProduct.Password != product.Password)
            {
                TempData["Error"] = "비밀번호가 올바르지 않아 삭제할 수 없습니다."; 
                return RedirectToAction(nameof(Delete), new { id }); 
            }
        }
        // 관리자이거나 비밀번호가 일치하면 삭제 진행
        var productToDelete = await _context.Products.FindAsync(id);
        if (productToDelete != null)
        {
            _context.Products.Remove(productToDelete);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Product ID 존재 여부 확인 헬퍼 메서드
    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    // GET: Products/Admin (관리자 로그인 페이지 표시)
    public IActionResult Admin()
    {
        return View();
    }
    
    // POST: Products/Admin (관리자 로그인 처리)
    [HttpPost]
    public IActionResult Admin(string id, string password) 
    {
        if (id == AdminId && password == AdminPassword) 
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            return RedirectToAction(nameof(Index)); 
        }
        ModelState.AddModelError("", "관리자 아이디 또는 비밀번호가 올바르지 않습니다."); 
        return View();
    }

    // GET: Products/Logout (관리자 로그아웃 처리)
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("IsAdmin"); // 세션에서 관리자 상태 제거
        return RedirectToAction(nameof(Index)); // Index 페이지로 리디렉션
    }
}