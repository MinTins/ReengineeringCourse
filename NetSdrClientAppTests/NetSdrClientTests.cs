using Moq;
using NetSdrClientApp;
using NetSdrClientApp.Networking;

namespace NetSdrClientAppTests;

public class NetSdrClientTests
{
    NetSdrClient _client;
    Mock<ITcpClient> _tcpMock;
    Mock<IUdpClient> _updMock;

    public NetSdrClientTests() { }

    [SetUp]
    public void Setup()
    {
        _tcpMock = new Mock<ITcpClient>();
        _tcpMock.Setup(tcp => tcp.Connect()).Callback(() =>
        {
            _tcpMock.Setup(tcp => tcp.Connected).Returns(true);
        });

        _tcpMock.Setup(tcp => tcp.Disconnect()).Callback(() =>
        {
            _tcpMock.Setup(tcp => tcp.Connected).Returns(false);
        });

        _tcpMock.Setup(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>())).Callback<byte[]>((bytes) =>
        {
            _tcpMock.Raise(tcp => tcp.MessageReceived += null, _tcpMock.Object, bytes);
        });

        _updMock = new Mock<IUdpClient>();

        _client = new NetSdrClient(_tcpMock.Object, _updMock.Object);
    }

    [Test]
    public async Task ConnectAsyncTest()
    {
        //act
        await _client.ConnectAsync();

        //assert
        _tcpMock.Verify(tcp => tcp.Connect(), Times.Once);
        _tcpMock.Verify(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>()), Times.Exactly(3));
    }

    [Test]
    public async Task DisconnectWithNoConnectionTest()
    {
        //act
        _client.Disconect();

        //assert
        //No exception thrown
        _tcpMock.Verify(tcp => tcp.Disconnect(), Times.Once);
    }

    [Test]
    public async Task DisconnectTest()
    {
        //Arrange 
        await ConnectAsyncTest();

        //act
        _client.Disconect();

        //assert
        //No exception thrown
        _tcpMock.Verify(tcp => tcp.Disconnect(), Times.Once);
    }

    [Test]
    public async Task StartIQNoConnectionTest()
    {

        //act
        await _client.StartIQAsync();

        //assert
        //No exception thrown
        _tcpMock.Verify(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>()), Times.Never);
        _tcpMock.VerifyGet(tcp => tcp.Connected, Times.AtLeastOnce);
    }

    [Test]
    public async Task StartIQTest()
    {
        //Arrange 
        await ConnectAsyncTest();

        //act
        await _client.StartIQAsync();

        //assert
        //No exception thrown
        _updMock.Verify(udp => udp.StartListeningAsync(), Times.Once);
        Assert.That(_client.IQStarted, Is.True);
    }

    [Test]
    public async Task StopIQTest()
    {
        //Arrange 
        await ConnectAsyncTest();

        //act
        await _client.StopIQAsync();

        //assert
        //No exception thrown
        _updMock.Verify(tcp => tcp.StopListening(), Times.Once);
        Assert.That(_client.IQStarted, Is.False);
    }

    [Test]
    public async Task StopIQNoConnectionTest()
    {
        // act
        await _client.StopIQAsync();

        // assert — без з'єднання StopListening не викликається
        _updMock.Verify(udp => udp.StopListening(), Times.Never);
        Assert.That(_client.IQStarted, Is.False);
    }

    [Test]
    public async Task ChangeFrequencyAsyncTest()
    {
        // Arrange
        await ConnectAsyncTest();

        // Act
        await _client.ChangeFrequencyAsync(20000000, 1);

        // Assert
        _tcpMock.Verify(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>()), Times.Exactly(4)); // 3 з connect + 1
    }

    [Test]
    public async Task ChangeFrequencyNoConnectionTest()
    {
        // Act
        await _client.ChangeFrequencyAsync(20000000, 1);

        // Assert
        _tcpMock.Verify(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>()), Times.Never);
    }

    [Test]
    public async Task StartIQSetsIQStartedTrueTest()
    {
        // Arrange
        await ConnectAsyncTest();

        // Act
        await _client.StartIQAsync();

        // Assert
        Assert.That(_client.IQStarted, Is.True);
    }

    [Test]
    public async Task StopIQSetsIQStartedFalseTest()
    {
        // Arrange
        await ConnectAsyncTest();
        await _client.StartIQAsync();

        // Act
        await _client.StopIQAsync();

        // Assert
        Assert.That(_client.IQStarted, Is.False);
    }
}

// ---- Lab 8: additional tests for coverage ≥80% on new code ----
public partial class NetSdrClientAdditionalTests
{
    NetSdrClient _client = null!;
    Mock<ITcpClient> _tcpMock = null!;
    Mock<IUdpClient> _udpMock = null!;

    [SetUp]
    public void AdditionalSetup()
    {
        _tcpMock = new Mock<ITcpClient>();
        _tcpMock.Setup(tcp => tcp.Connect()).Callback(() =>
            _tcpMock.Setup(tcp => tcp.Connected).Returns(true));
        _tcpMock.Setup(tcp => tcp.Disconnect()).Callback(() =>
            _tcpMock.Setup(tcp => tcp.Connected).Returns(false));
        _tcpMock.Setup(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>())).Callback<byte[]>(bytes =>
            _tcpMock.Raise(tcp => tcp.MessageReceived += null, _tcpMock.Object, bytes));
        _udpMock = new Mock<IUdpClient>();
        _client = new NetSdrClient(_tcpMock.Object, _udpMock.Object);
    }

    [Test]
    public async Task ConnectAsync_AlreadyConnected_DoesNotConnectAgain()
    {
        await _client.ConnectAsync();
        await _client.ConnectAsync(); // другий виклик
        _tcpMock.Verify(tcp => tcp.Connect(), Times.Once);
    }

    [Test]
    public async Task StartIQ_ThenStop_IQStartedIsFalse()
    {
        await _client.ConnectAsync();
        await _client.StartIQAsync();
        Assert.That(_client.IQStarted, Is.True);
        await _client.StopIQAsync();
        Assert.That(_client.IQStarted, Is.False);
    }

    [Test]
    public async Task ChangeFrequency_ZeroHz_SendsMessage()
    {
        await _client.ConnectAsync();
        await _client.ChangeFrequencyAsync(0, 0);
        _tcpMock.Verify(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>()), Times.Exactly(4));
    }

    [Test]
    public async Task ChangeFrequency_MaxHz_SendsMessage()
    {
        await _client.ConnectAsync();
        await _client.ChangeFrequencyAsync(long.MaxValue, 1);
        _tcpMock.Verify(tcp => tcp.SendMessageAsync(It.IsAny<byte[]>()), Times.Exactly(4));
    }
}
