#include "Game.h"
#include <comdef.h>

using namespace Microsoft::WRL;

Graphics::Graphics() {
	ComPtr<IDXGIFactory1> dxgiFactory;
	ComPtr<IDXGIAdapter1> dxgiAdapter;
	ComPtr<IDXGIDevice1> dxgiDevice;
	ComPtr<ID3D11Device> graphicsDevice;
	ComPtr<ID3D11DeviceContext> immediateContext;
	D3D_FEATURE_LEVEL featureLevel = D3D_FEATURE_LEVEL();

	UINT creationFlags = 0;
#if defined(_DEBUG)
	creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif
	// デバイス、デバイスコンテキスト、スワップチェーンを作成
	const auto hr = D3D11CreateDevice(
		nullptr, D3D_DRIVER_TYPE_HARDWARE, 0, creationFlags, NULL, 0, D3D11_SDK_VERSION,
		&graphicsDevice, &featureLevel, &immediateContext);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}
	if (FAILED(graphicsDevice.As(&dxgiDevice))) {
		throw _com_error(hr);
	}
	ComPtr<IDXGIAdapter> adapter;
	if (FAILED(dxgiDevice->GetAdapter(&adapter))) {
		throw _com_error(hr);
	}
	if (FAILED(adapter.As(&dxgiAdapter))) {
		throw _com_error(hr);
	}
	if (FAILED(dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory)))) {
		throw _com_error(hr);
	}

	this->dxgiFactory = dxgiFactory;
	this->dxgiAdapter = dxgiAdapter;
	this->dxgiDevice = dxgiDevice;
	this->graphicsDevice = graphicsDevice;
	this->immediateContext = immediateContext;
	this->featureLevel = featureLevel;
}

IDXGIFactory1* Graphics::GetDXGI_Factory()
{
	return dxgiFactory.Get();
}

ID3D11Device* Graphics::GetDevice()
{
	return graphicsDevice.Get();
}

ID3D11DeviceContext* Graphics::GetDeviceContext()
{
	return immediateContext.Get();
}