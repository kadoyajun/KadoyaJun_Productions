#include "Game.h"
#include <comdef.h>

using namespace Microsoft::WRL;

SwapChain::SwapChain(
	std::shared_ptr<Graphics> graphics, std::shared_ptr<MainWindow> window,
DXGI_FORMAT swapChainFormat, DXGI_FORMAT depthStencilFormat)
{
	HRESULT hr = S_OK;
	ComPtr<IDXGISwapChain> swapChain;
	ComPtr<ID3D11RenderTargetView> renderTargetView;
	ComPtr<ID3D11ShaderResourceView> renderTargetResourceView;
	ComPtr<ID3D11DepthStencilView> depthStencilView;
	ComPtr<ID3D11ShaderResourceView> depthStencilResourceView;

	DXGI_SWAP_CHAIN_DESC swapChainDesc = {};
	swapChainDesc.BufferDesc.Width = window->GetWidth();
	swapChainDesc.BufferDesc.Height = window->GetHeight();
	swapChainDesc.BufferDesc.RefreshRate = { 60, 1 };
	swapChainDesc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
	swapChainDesc.SampleDesc = { 1, 0 };
	swapChainDesc.BufferUsage =
		DXGI_USAGE_RENDER_TARGET_OUTPUT |
		DXGI_USAGE_SHADER_INPUT;
	swapChainDesc.BufferCount = 2;
	swapChainDesc.OutputWindow = window->GetHandle();
	swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
	swapChainDesc.Windowed = TRUE;
	hr = graphics->GetDXGI_Factory()->CreateSwapChain(graphics->GetDevice(), &swapChainDesc, &swapChain);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}

	ComPtr<ID3D11Texture2D> backBuffer;
	hr = swapChain->GetBuffer(0, IID_PPV_ARGS(&backBuffer));
	if (FAILED(hr)) {
		throw _com_error(hr);
	}
	hr = graphics->GetDevice()->CreateRenderTargetView(backBuffer.Get(), NULL, &renderTargetView);
	if (FAILED(hr) || renderTargetView == NULL) {
		throw _com_error(hr);
	}
	hr = graphics->GetDevice()->CreateShaderResourceView(
		backBuffer.Get(),
		NULL,
		&renderTargetResourceView);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}

	DXGI_FORMAT textureFormat = depthStencilFormat;
	DXGI_FORMAT resourceFormat = depthStencilFormat;
	switch (depthStencilFormat)
	{
	case DXGI_FORMAT_D16_UNORM:
		textureFormat = DXGI_FORMAT_R16_TYPELESS;
		resourceFormat = DXGI_FORMAT_R16_UNORM;
		break;
	case DXGI_FORMAT_D24_UNORM_S8_UINT:
		textureFormat = DXGI_FORMAT_R24G8_TYPELESS;
		resourceFormat = DXGI_FORMAT_R24_UNORM_X8_TYPELESS;
		break;
	case DXGI_FORMAT_D32_FLOAT:
		textureFormat = DXGI_FORMAT_R32_TYPELESS;
		resourceFormat = DXGI_FORMAT_R32_FLOAT;
		break;
	case DXGI_FORMAT_D32_FLOAT_S8X24_UINT:
		textureFormat = DXGI_FORMAT_R32G8X24_TYPELESS;
		resourceFormat = DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS;
		break;
	}
	ComPtr<ID3D11Texture2D> depthStencil = nullptr;
	D3D11_TEXTURE2D_DESC depthStencilDesc = {};
	depthStencilDesc.Width = swapChainDesc.BufferDesc.Width;
	depthStencilDesc.Height = swapChainDesc.BufferDesc.Height;
	depthStencilDesc.MipLevels = 1;
	depthStencilDesc.ArraySize = 1;
	depthStencilDesc.Format = textureFormat;
	depthStencilDesc.SampleDesc = swapChainDesc.SampleDesc;
	depthStencilDesc.Usage = D3D11_USAGE_DEFAULT;
	depthStencilDesc.BindFlags =
		D3D11_BIND_DEPTH_STENCIL |
		D3D11_BIND_SHADER_RESOURCE;
	depthStencilDesc.CPUAccessFlags = 0;
	depthStencilDesc.MiscFlags = 0;
	hr = graphics->GetDevice()->CreateTexture2D(&depthStencilDesc, NULL, &depthStencil);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}
	D3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc = {};
	depthStencilViewDesc.Format = depthStencilFormat;
	if (depthStencilDesc.SampleDesc.Count > 0) {
		depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2DMS;
	}
	else {
		depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
		depthStencilViewDesc.Texture2D.MipSlice = 0;
	}
	depthStencilViewDesc.Texture2D.MipSlice = 0;
	hr = graphics->GetDevice()->CreateDepthStencilView(depthStencil.Get(), &depthStencilViewDesc, &depthStencilView);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}
	D3D11_SHADER_RESOURCE_VIEW_DESC depthStencilResourceViewDesc = {};
	depthStencilResourceViewDesc.Format = resourceFormat;
	if (depthStencilDesc.SampleDesc.Count > 0) {
		depthStencilResourceViewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2DMS;
	}
	else {
		depthStencilResourceViewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
		depthStencilResourceViewDesc.Texture2D.MostDetailedMip = 0;
		depthStencilResourceViewDesc.Texture2D.MipLevels = 1;
	}
	hr = graphics->GetDevice()->CreateShaderResourceView(
		depthStencil.Get(),
		&depthStencilResourceViewDesc,
		&depthStencilResourceView);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}


	this->swapChain = swapChain;
	this->renderTargetView = renderTargetView;
	this->renderTargetResourceView = renderTargetResourceView;
	this->depthStencilView = depthStencilView;
	this->depthStencilResourceView = depthStencilResourceView;
}

ID3D11RenderTargetView* SwapChain::GetRenderTargetView()
{
	return renderTargetView.Get();
}

ID3D11DepthStencilView* SwapChain::GetDepthStencilView()
{
	return depthStencilView.Get();
}

void SwapChain::Present(UINT syncInterval)
{
	const auto hr = swapChain->Present(syncInterval, 0);
	if (FAILED(hr))
	{
		throw _com_error(hr);
	}
}