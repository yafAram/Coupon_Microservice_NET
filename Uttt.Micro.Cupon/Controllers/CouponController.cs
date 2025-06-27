using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uttt.Micro.Cupon.Data;
using Uttt.Micro.Cupon.Models;
using Uttt.Micro.Cupon.Models.Dto;

namespace Uttt.Micro.Cupon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private ResponseDto _response;
        private IMapper _mapper;

        public CouponController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        /// <summary>
        /// Obtiene todos los cupones
        /// </summary>
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _dbContext.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Obtiene un cupón por ID
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon obj = _dbContext.Coupons.FirstOrDefault(u => u.CouponId == id);
                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found";
                    return _response;
                }
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Obtiene un cupón por código
        /// </summary>
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon coupon = _dbContext.Coupons.
                    FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());

                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found";
                    return _response;
                }

                // Validar si el cupón está activo y dentro del rango de fechas
                if (!coupon.StateCoupon)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon is not active";
                    return _response;
                }

                if (DateTime.Now < coupon.DateInt || DateTime.Now > coupon.DateEnd)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon is not valid for current date";
                    return _response;
                }

                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Genera un código de cupón único
        /// </summary>
        [HttpGet]
        [Route("GenerateCode")]
        [Authorize(Roles = "ADMINISTRATOR,SALES")]
        public ResponseDto GenerateCode()
        {
            try
            {
                string newCode;
                do
                {
                    newCode = GenerateUniqueCode();
                } while (_dbContext.Coupons.Any(c => c.CouponCode == newCode));

                _response.Result = new { CouponCode = newCode };
                _response.Message = "Coupon code generated successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Valida si un código de cupón está disponible
        /// </summary>
        [HttpGet]
        [Route("ValidateCode/{code}")]
        [Authorize(Roles = "ADMINISTRATOR,SALES")]
        public ResponseDto ValidateCode(string code)
        {
            try
            {
                var exists = _dbContext.Coupons.Any(c => c.CouponCode.ToLower() == code.ToLower());
                _response.Result = new { IsAvailable = !exists, Code = code };
                _response.Message = exists ? "Coupon code already exists" : "Coupon code is available";
                _response.IsSuccess = !exists;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Crea un nuevo cupón
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMINISTRATOR,SALES")]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                // Si no se proporciona código, generar uno automáticamente
                if (string.IsNullOrEmpty(couponDto.CouponCode))
                {
                    string newCode;
                    do
                    {
                        newCode = GenerateUniqueCode();
                    } while (_dbContext.Coupons.Any(c => c.CouponCode == newCode));

                    couponDto.CouponCode = newCode;
                }

                // Validar que no exista un cupón con el mismo código
                var existingCoupon = _dbContext.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == couponDto.CouponCode.ToLower());
                if (existingCoupon != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon code already exists";
                    return _response;
                }

                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _dbContext.Coupons.Add(obj);
                _dbContext.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.Message = "Coupon created successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Genera un código único para cupones
        /// </summary>
        private string GenerateUniqueCode()
        {
            var random = new Random();
            var prefixes = new[] { "SAVE", "DISC", "DEAL", "PROMO", "SPECIAL" };
            var prefix = prefixes[random.Next(prefixes.Length)];
            var number = random.Next(10, 999);
            var suffix = random.Next(10, 99);

            return $"{prefix}{number}{suffix}";
        }

        /// <summary>
        /// Actualiza un cupón existente
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "ADMINISTRATOR")]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                // Buscar el cupón existente en la base de datos
                var existingCoupon = _dbContext.Coupons.FirstOrDefault(c => c.CouponId == couponDto.CouponId);
                if (existingCoupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found";
                    return _response;
                }

                // Actualizar las propiedades del objeto existente (ya tracked por EF)
                existingCoupon.CouponCode = couponDto.CouponCode;
                existingCoupon.DiscountAmount = couponDto.DiscountAmount;
                existingCoupon.MinAmount = couponDto.MinAmount;
                existingCoupon.AmountType = couponDto.AmountType;
                existingCoupon.LimitUse = couponDto.LimitUse;
                existingCoupon.DateInt = couponDto.DateInt;
                existingCoupon.DateEnd = couponDto.DateEnd;
                existingCoupon.Category = couponDto.Category;
                existingCoupon.StateCoupon = couponDto.StateCoupon;

                // Guardar cambios (EF detecta automáticamente los cambios)
                _dbContext.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(existingCoupon);
                _response.Message = "Coupon updated successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Elimina un cupón
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMINISTRATOR")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = _dbContext.Coupons.FirstOrDefault(u => u.CouponId == id);
                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found";
                    return _response;
                }

                _dbContext.Coupons.Remove(obj);
                _dbContext.SaveChanges();
                _response.Message = "Coupon deleted successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}