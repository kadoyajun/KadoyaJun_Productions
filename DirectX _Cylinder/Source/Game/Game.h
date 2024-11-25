#pragma once

#include <string>
#include <memory>
#include <Windows.h>
#include <d3d11.h>
#include <DirectXMath.h>
#include <wrl/client.h>


#define SAFE_RELEASE(p) if ((p) != nullptr) { (p)->Release(); (p) = nullptr; }

struct WindowSettings
{
	std::wstring title = L"タイトル";
	int screenWidth = 640;
	int screenHeight = 480;
};

// メイン ウィンドウの表示
class MainWindow
{
public:
	MainWindow(const WindowSettings& settings = WindowSettings());
	virtual ~MainWindow() = default;
	int GetWidth()const;
	int GetHeight()const;
	HWND GetHandle()const;
private:
	static LRESULT CALLBACK WindowProc(HWND windowHandle, UINT message, WPARAM wParam, LPARAM lParam);

	WindowSettings settings;
	HWND handle = NULL;
};

class Graphics final {
public:
	Graphics();
	~Graphics() = default;

	IDXGIFactory1* GetDXGI_Factory();
	ID3D11Device* GetDevice();
	ID3D11DeviceContext* GetDeviceContext();
private:
	Microsoft::WRL::ComPtr<IDXGIFactory1> dxgiFactory = nullptr;
	Microsoft::WRL::ComPtr<IDXGIAdapter1> dxgiAdapter = nullptr;
	Microsoft::WRL::ComPtr<IDXGIDevice1> dxgiDevice = nullptr;
	Microsoft::WRL::ComPtr<ID3D11Device> graphicsDevice = nullptr;
	Microsoft::WRL::ComPtr<ID3D11DeviceContext> immediateContext = nullptr;
	D3D_FEATURE_LEVEL featureLevel = {};
};

class SwapChain final {
public:
	SwapChain(
		std::shared_ptr<Graphics> graphics, std::shared_ptr<MainWindow> window,
		DXGI_FORMAT swapChainFormat = DXGI_FORMAT_R8G8B8A8_UNORM,
		DXGI_FORMAT depthStencilFormat = DXGI_FORMAT_D24_UNORM_S8_UINT);
	~SwapChain() = default;

	ID3D11RenderTargetView* GetRenderTargetView();
	ID3D11DepthStencilView* GetDepthStencilView();

	void Present(UINT syncInterval);

	explicit SwapChain(const SwapChain&) = delete;
	explicit SwapChain(SwapChain&&) = delete;
	SwapChain& operator=(const SwapChain&) = delete;
	SwapChain& operator=(SwapChain&&) = delete;
private:
	std::unique_ptr<MainWindow> window;
	std::unique_ptr<Graphics> graphics;
	Microsoft::WRL::ComPtr<IDXGISwapChain> swapChain = nullptr;
	Microsoft::WRL::ComPtr<ID3D11RenderTargetView> renderTargetView = {};
	Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> renderTargetResourceView = nullptr;
	const DXGI_FORMAT depthStencilFormat = DXGI_FORMAT_D24_UNORM_S8_UINT;
	Microsoft::WRL::ComPtr<ID3D11DepthStencilView> depthStencilView = nullptr;
	Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> depthStencilResourceView = nullptr;
};

// アプリケーション全体を表す
class Game {
public:
	Game() noexcept = default;
	virtual ~Game() = default;

	int Run(const WindowSettings& settings = WindowSettings()) noexcept;

private:
	std::shared_ptr<MainWindow> window;
	std::shared_ptr<Graphics> graphics;
	std::unique_ptr<SwapChain> swapChain;
	FLOAT clearColor[4] = { 53 / 255.0f, 70 / 255.0f, 166 / 255.0f, 1.0f };

	D3D11_VIEWPORT viewport = {};
};


struct VertexPosition
{
	DirectX::XMFLOAT3 position;

	static constexpr D3D11_INPUT_ELEMENT_DESC inputElementDescs[] = {
		{ "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
	};
};

struct VertexPositionColor
{
	DirectX::XMFLOAT3 position;
	DirectX::XMFLOAT4 color;

	static constexpr D3D11_INPUT_ELEMENT_DESC inputElementDescs[] = {
		{ "POSITION", 0,    DXGI_FORMAT_R32G32B32_FLOAT, 0,                            0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
		{ "COLOR",    0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0 },
	};
};

struct VertexPositionNormal
{
	DirectX::XMFLOAT3 position;	
	DirectX::XMFLOAT3 normal;

	static constexpr D3D11_INPUT_ELEMENT_DESC inputElementDescs[] = {
		{ "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0,                            0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
		{ "NORMAL",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0 },
	};
};

class BasicVertexShader
{
private:
	Microsoft::WRL::ComPtr<ID3D11VertexShader> shader = nullptr;

public:
	BasicVertexShader(ID3D11Device* graphicsDevice);
	~BasicVertexShader(){}

	ID3D11VertexShader* GetNativePointer();
	const BYTE* GetBytecode();
	SIZE_T GetBytecodeLength();
};

class BasicGeometryShader
{
private:
	Microsoft::WRL::ComPtr<ID3D11GeometryShader> shader = nullptr;

public:
	BasicGeometryShader(ID3D11Device* graphicsDevice);
	~BasicGeometryShader(){}

	ID3D11GeometryShader* GetNativePointer();
};

class BasicPixelShader
{
private:
	Microsoft::WRL::ComPtr<ID3D11PixelShader> shader = nullptr;

public:
	BasicPixelShader(ID3D11Device* graphicsDevice);
	~BasicPixelShader() {}

	ID3D11PixelShader* GetNativePointer();
};

class VertexBuffer
{
	Microsoft::WRL::ComPtr<ID3D11Buffer> buffer = nullptr;

public:
	VertexBuffer(ID3D11Device* graphicsDevice, UINT byteWidth);
	~VertexBuffer() {}

	ID3D11Buffer* GetNativePointer();
	void SetData(void* data);
};

class IndexBuffer
{
	Microsoft::WRL::ComPtr<ID3D11Buffer> buffer = nullptr;

public:
	IndexBuffer(ID3D11Device* graphicsDevice, UINT indexCount);
	~IndexBuffer(){}

	ID3D11Buffer* GetNativePointer();
	void SetData(UINT32* data);
};

class ConstantBuffer
{
	Microsoft::WRL::ComPtr<ID3D11Buffer> buffer = nullptr;

public:
	ConstantBuffer(ID3D11Device* graphicsDevice, UINT byteWidth);
	~ConstantBuffer(){}

	ID3D11Buffer* GetNativePointer();
	void SetData(void* data);
};

class InputLayout
{
	Microsoft::WRL::ComPtr<ID3D11InputLayout> inputLayout = nullptr;

public:
	InputLayout(
		ID3D11Device* graphicsDevice,
		const D3D11_INPUT_ELEMENT_DESC* inputElementDescs, UINT numElements,
		const void* shaderBytecodeWithInputSignature, SIZE_T bytecodeLength);
	~InputLayout(){}

	ID3D11InputLayout* GetNativePointer();
};