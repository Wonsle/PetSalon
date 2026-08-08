using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using PetSalon.Models.EntityModels;

namespace PetSalon.Web.Tests.Authentication;

public sealed class AuthenticationModelTests
{
    private static PetSalonContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PetSalonContext>()
            .UseSqlServer("Server=localhost;Database=PetSalonModelTests;User Id=sa;Password=ModelTests-Only!;TrustServerCertificate=True")
            .Options;

        return new PetSalonContext(options);
    }

    [Fact]
    public void ScuserRequiresUniqueUserNameAndSafeStateDefaults()
    {
        using var context = CreateContext();
        var entity = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Scuser));

        Assert.NotNull(entity);
        var userName = entity.FindProperty(nameof(Scuser.UserName));
        Assert.NotNull(userName);
        Assert.False(userName.IsNullable);
        Assert.Equal("Latin1_General_100_CI_AS", userName.GetCollation());
        Assert.Contains(entity.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(Scuser.UserName)]));

        Assert.Equal(false, entity.FindProperty(nameof(Scuser.MustChangePassword))?.GetDefaultValue());
        Assert.Equal(true, entity.FindProperty(nameof(Scuser.IsActive))?.GetDefaultValue());
    }

    [Fact]
    public void ScuserRoleHasForeignKeysAndUniqueUserRolePair()
    {
        using var context = CreateContext();
        var roleEntity = context.Model.FindEntityType(typeof(Scrole));
        var entity = context.Model.FindEntityType(typeof(ScuserRole));

        Assert.NotNull(roleEntity);
        Assert.Contains(roleEntity.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(Scrole.RoleName)]));
        Assert.NotNull(entity);
        Assert.Contains(entity.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(Scuser));
        Assert.Contains(entity.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(Scrole));
        Assert.Contains(entity.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(ScuserRole.ScuserId), nameof(ScuserRole.RoleId)]));
    }

    [Fact]
    public void PermissionModelRequiresUniqueCodeAndUniqueRolePermissionPair()
    {
        using var context = CreateContext();
        var model = context.GetService<IDesignTimeModel>().Model;
        var permission = model.FindEntityType(typeof(Scpermission));
        var rolePermission = model.FindEntityType(typeof(ScrolePermission));

        Assert.NotNull(permission);
        var code = permission.FindProperty(nameof(Scpermission.PermissionCode));
        Assert.NotNull(code);
        Assert.False(code.IsNullable);
        Assert.Equal("Latin1_General_100_CI_AS", code.GetCollation());
        Assert.Equal(true, permission.FindProperty(nameof(Scpermission.IsActive))?.GetDefaultValue());
        Assert.Contains(permission.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(Scpermission.PermissionCode)]));

        Assert.NotNull(rolePermission);
        Assert.Contains(rolePermission.GetForeignKeys(), key =>
            key.PrincipalEntityType.ClrType == typeof(Scrole));
        Assert.Contains(rolePermission.GetForeignKeys(), key =>
            key.PrincipalEntityType.ClrType == typeof(Scpermission));
        Assert.Contains(rolePermission.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(ScrolePermission.RoleId), nameof(ScrolePermission.PermissionId)]));
    }
}
