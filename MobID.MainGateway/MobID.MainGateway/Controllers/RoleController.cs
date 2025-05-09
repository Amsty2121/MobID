﻿using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService) => _roleService = roleService;

        [HttpPost("add")]
        public async Task<IActionResult> AddRole([FromQuery] string roleName, [FromQuery] string description, CancellationToken ct)
        {
            try
            {
                var result = await _roleService.AddRole(roleName, description, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(Guid roleId, CancellationToken ct)
        {
            try
            {
                bool success = await _roleService.DeleteRole(roleId, ct);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(Guid roleId, CancellationToken ct)
        {
            try
            {
                var role = await _roleService.GetRoleById(roleId, ct);
                return role == null ? NotFound() : Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("byname/{roleName}")]
        public async Task<IActionResult> GetRoleByName(string roleName, CancellationToken ct)
        {
            try
            {
                var role = await _roleService.GetRoleByName(roleName, ct);
                return role == null ? NotFound() : Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles(CancellationToken ct)
        {
            try
            {
                var roles = await _roleService.GetAllRoles(ct);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetRolesPaged([FromQuery] Models.Dtos.PagedRequest request, CancellationToken ct)
        {
            try
            {
                var pagedRoles = await _roleService.GetRolesPaged(request, ct);
                return Ok(pagedRoles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
