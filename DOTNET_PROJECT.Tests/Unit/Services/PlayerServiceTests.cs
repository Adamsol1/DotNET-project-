using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DOTNET_PROJECT.Application;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Domain.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace DOTNET_PROJECT.Tests.Unit.Services
{
    public class PlayerServiceTests
    {
        private readonly AutoMocker _mocker;

        public PlayerServiceTests() => _mocker = new AutoMocker();

        private PlayerService NewSut()
        {
            // Knyt repoet til UoW én gang
            var uow = _mocker.GetMock<IUnitOfWork>();
            var repo = _mocker.GetMock<IPlayerCharacterRepository>();
            uow.SetupGet(x => x.PlayerCharacterRepository).Returns(repo.Object);

            // No-op standard for transaksjoner
            uow.Setup(x => x.BeginAsync()).Returns(Task.CompletedTask);
            uow.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
            uow.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
            uow.Setup(x => x.RollBackAsync()).Returns(Task.CompletedTask);

            return _mocker.CreateInstance<PlayerService>();
        }

        private static PlayerCharacter Pc(int id = 1, int health = 100) =>
            new PlayerCharacter { Id = id, Name = "Hero", UserId = 1, CurrentStoryNodeId = 10, Health = health };

        [Fact]
        public async Task createPlayerCharacter_success()
        {
            // Arrange
            var sut = NewSut();
            var req = new CreatePlayerCharacterDto { Name = "Hero", UserId = 1, CurrentStoryNodeId = 10 };
            _mocker.GetMock<IPlayerCharacterRepository>()
                   .Setup(r => r.Create(It.IsAny<PlayerCharacter>()))
                   .Returns(Task.CompletedTask);

            // Act
            var dto = await sut.CreatePlayerCharacter(req);

            // Assert
            Assert.Equal("Hero", dto.Name);
            Assert.Equal(100, dto.Health);
            _mocker.GetMock<IPlayerCharacterRepository>()
                   .Verify(r => r.Create(It.Is<PlayerCharacter>(p => p.Name == "Hero" && p.Health == 100)), Times.Once);
            _mocker.GetMock<IUnitOfWork>().Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPlayerCharacterById_Success()
        {
            // Arrange
            var sut = NewSut();
            _mocker.GetMock<IPlayerCharacterRepository>()
                   .Setup(r => r.GetById(7))
                   .ReturnsAsync(Pc(id: 7, health: 42));

            // Act
            var dto = await sut.GetPlayerCharacterById(7);

            // Assert
            Assert.Equal(7, dto.Id);
            Assert.Equal(42, dto.Health);
        }

        [Fact]
        public async Task GetPlayerCharacterById_ThrowsException()
        {
            // Arrange
            var sut = NewSut();
            _mocker.GetMock<IPlayerCharacterRepository>()
                   .Setup(r => r.GetById(123))
                   .ReturnsAsync((PlayerCharacter)null);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.GetPlayerCharacterById(123));
            Assert.Contains("Player character not found", ex.Message);
        }

        [Fact]
        public async Task DeleteCharacter_Success()
        {
            // Arrange
            var sut = NewSut();
            _mocker.GetMock<IPlayerCharacterRepository>()
                   .Setup(r => r.GetById(11))
                   .ReturnsAsync(Pc(id: 11));
            _mocker.GetMock<IPlayerCharacterRepository>()
                   .Setup(r => r.Delete(11))
                   .Returns(Task.CompletedTask);

            // Act
            var ok = await sut.DeleteCharacter(11);

            // Assert
            Assert.True(ok);
            _mocker.GetMock<IPlayerCharacterRepository>().Verify(r => r.Delete(11), Times.Once);
            _mocker.GetMock<IUnitOfWork>().Verify(u => u.CommitAsync(), Times.Once);
        }

        [Theory]
        [InlineData(50, 30, 30)]
        [InlineData(50, -10, 0)]
        public async Task SetHealth_Success(int initial, int newHealth, int expected)
        {
            // Arrange
            var sut = NewSut();
            var repo = _mocker.GetMock<IPlayerCharacterRepository>();
            repo.Setup(r => r.GetById(1)).ReturnsAsync(Pc(id: 1, health: initial));
            repo.Setup(r => r.Update(It.IsAny<PlayerCharacter>())).Returns(Task.CompletedTask);

            // Act
            var result = await sut.SetHealth(1, newHealth);

            // Assert
            Assert.Equal(expected, result);
            repo.Verify(r => r.Update(It.Is<PlayerCharacter>(p => p.Health == expected)), Times.Once);
        }

        [Theory]
        [InlineData(50, 10, 60)]
        [InlineData(3, -10, 0)]
        public async Task ModifyHealth_Success(int initial, int delta, int expected)
        {
            // Arrange
            var sut = NewSut();
            var repo = _mocker.GetMock<IPlayerCharacterRepository>();
            repo.Setup(r => r.GetById(2)).ReturnsAsync(Pc(id: 2, health: initial));
            repo.Setup(r => r.Update(It.IsAny<PlayerCharacter>())).Returns(Task.CompletedTask);

            // Act
            var result = await sut.ModifyHealth(2, delta);

            // Assert
            Assert.Equal(expected, result);
            repo.Verify(r => r.Update(It.Is<PlayerCharacter>(p => p.Health == expected)), Times.Once);
            _mocker.GetMock<IUnitOfWork>().Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}