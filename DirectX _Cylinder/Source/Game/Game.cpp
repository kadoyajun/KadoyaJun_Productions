//https://learn.microsoft.com/ja-jp/windows-hardware/drivers/display/pipelines-for-direct3d-version-11

#include <DirectXMath.h>
#include <DirectXColors.h>
#include "Game.h"
#include "BasicVertexShader.h"
#include "BasicGeometryShader.h"
#include "BasicPixelShader.h"
#include <timeapi.h>
#pragma comment(lib, "Winmm.lib")
#include <new>
#include <comdef.h>

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace std;

int Game::Run(const WindowSettings& settings) noexcept
{
	try {
		window.reset(new MainWindow(settings)); 
		graphics.reset(new Graphics()); 
		swapChain.reset(new SwapChain(graphics, window));
	}
	catch (const std::exception& error) {
		OutputDebugStringA("ERROR: ");
		OutputDebugStringA(error.what());
		OutputDebugStringA("\n");
		MessageBoxW(NULL, L"アプリケーションを初期化できませんでした。", L"ERROR", MB_OK);
		return 0;
	}

	viewport.Width = static_cast<FLOAT>(window->GetWidth());
	viewport.Height = static_cast<FLOAT>(window->GetHeight());
	viewport.MinDepth = 0.0f;
	viewport.MaxDepth = 1.0f;
	viewport.TopLeftX = 0.0f;
	viewport.TopLeftY = 0.0f;
	
	//円柱
	constexpr int splitCount = 64; //分割数
	VertexPositionNormal vertices[(splitCount + 1) * 2 + splitCount * 4] = {};

	//表面
	vertices[0] = VertexPositionNormal{ {0.0f,0.0f,1.0f},{0.0f,0.0f,1.0f} };
	for (int i = 0; i < splitCount; i++) {
		float vertex_radian = i * (XM_2PI / splitCount); //頂点のラジアン
		vertices[i + 1] = VertexPositionNormal{ 
			{ XMScalarCos(vertex_radian),XMScalarSin(vertex_radian),1.0f },
			{0.0f,0.0f,1.0f} };
	}

	//裏面
	vertices[splitCount + 1] = VertexPositionNormal{ {0.0f,0.0f,-1.0f},{0.0f,0.0f,-1.0f} };
	for (int i = 0; i < splitCount; i++) {
		float vertex_radian = -i * (XM_2PI / splitCount); //頂点のラジアン
		vertices[splitCount + 2 + i] = VertexPositionNormal{ 
			{ XMScalarCos(vertex_radian),XMScalarSin(vertex_radian),-1.0f },
			{0.0f,0.0f,-1.0f} };
	}

	int circleVertexCount = (splitCount + 1) * 2;//表面と裏面に使用した頂点数

	//側面
	for (int i = 0; i < splitCount; i++) {
		XMVECTOR p0 = XMVectorSet(
			vertices[i + 1].position.x, 
			vertices[i + 1].position.y, 
			vertices[i + 1].position.z, 
			1.0f);
		XMVECTOR p1 = XMVectorSet(
			vertices[(i + 1) % splitCount + 1].position.x, 
			vertices[(i + 1) % splitCount + 1].position.y, 
			vertices[(i + 1) % splitCount + 1].position.z, 
			1.0f);
		XMVECTOR p2 = XMVectorSet(
			vertices[i + 1].position.x, 
			vertices[i + 1].position.y, 
			-1.0f, 
			1.0f);
		XMVECTOR p3 = XMVectorSet(
			vertices[(i + 1) % splitCount + 1].position.x, 
			vertices[(i + 1) % splitCount + 1].position.y, 
			-1.0f, 
			1.0f);

		XMVECTOR n0 = p0;
		XMVECTOR n1 = p1;
		XMVECTOR n2 = p2;
		XMVECTOR n3 = p3;

		n0 = XMVectorSetZ(n0, 0.0f);
		n1 = XMVectorSetZ(n1, 0.0f);
		n2 = XMVectorSetZ(n2, 0.0f);
		n3 = XMVectorSetZ(n3, 0.0f);

		n0 = XMVector3Normalize(n0);
		n1 = XMVector3Normalize(n1);
		n2 = XMVector3Normalize(n2);
		n3 = XMVector3Normalize(n3);

		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 0].position, p0);
		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 1].position, p1);
		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 2].position, p2);
		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 3].position, p3);
		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 0].normal, n0);
		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 1].normal, n1);
		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 2].normal, n2);
		XMStoreFloat3(&vertices[circleVertexCount + 4 * i + 3].normal, n3);
	}

	constexpr int circleIndicesCount = splitCount * 3 * 2;//表面と裏面に使用したインデックス

	UINT32 indices[circleIndicesCount + splitCount * 6] = {};
	for (int i = 0; i < splitCount; i++) {
		indices[3 * i] = 0;
		indices[3 * i + 1] = i + 1;
		indices[3 * i + 2] = (i + 1) % splitCount + 1;
		indices[3 * splitCount + 3 * i] = splitCount + 1;
		indices[3* splitCount + 3 * i + 1] = splitCount + 1 + i + 1;
		indices[3* splitCount + 3 * i + 2] = splitCount + 1 + (i + 1) % splitCount + 1;
	}
	for (int i = 0; i < splitCount; i++) {
		indices[circleIndicesCount + 6 * i + 0] = circleVertexCount + 4 * i + 2;
		indices[circleIndicesCount + 6 * i + 1] = circleVertexCount + 4 * i + 1;
		indices[circleIndicesCount + 6 * i + 2] = circleVertexCount + 4 * i + 0;
		indices[circleIndicesCount + 6 * i + 3] = circleVertexCount + 4 * i + 1;
		indices[circleIndicesCount + 6 * i + 4] = circleVertexCount + 4 * i + 2;
		indices[circleIndicesCount + 6 * i + 5] = circleVertexCount + 4 * i + 3;
	}

	const UINT indexCount = _countof(indices);

	struct MatricesPerFrame {
		DirectX::XMFLOAT4X4 worldMatrix;
		DirectX::XMFLOAT4X4 viewMatrix;
		DirectX::XMFLOAT4X4 projectionMatrix;
		DirectX::XMFLOAT4X4 worldViewProjectionMatrix;

		DirectX::XMFLOAT4 viewPosition = DirectX::XMFLOAT4(0, 0, -5, 1);

		DirectX::XMFLOAT4 lightPosition = DirectX::XMFLOAT4(1.0f, 2.0f, -2.0f, 1.0f);

		DirectX::XMFLOAT4 materialDiffuse = DirectX::XMFLOAT4(1, 1, 0, 1);

		DirectX::XMFLOAT3 materialSpecularColor = DirectX::XMFLOAT3(1, 1, 1);
		float materialSpecularPower = 1;
	};

	// バッファー
	VertexBuffer* vertexBuffer = nullptr;
	IndexBuffer* indexBuffer = nullptr;
	ConstantBuffer* constantBuffer = nullptr;
	// シェーダー
	BasicVertexShader* vertexShader = nullptr;
	BasicGeometryShader* geometryShader = nullptr;
	BasicPixelShader* pixelShader = nullptr;
	// 入力レイアウト
	InputLayout* inputLayout = nullptr;

	try {
		// バッファーを作成
		vertexBuffer = new VertexBuffer(graphics->GetDevice(), sizeof vertices);
		vertexBuffer->SetData(vertices);
		indexBuffer = new IndexBuffer(graphics->GetDevice(), ARRAYSIZE(indices));
		indexBuffer->SetData(indices);
		constantBuffer = new ConstantBuffer(graphics->GetDevice(), sizeof(MatricesPerFrame));
		// シェーダーを作成
		vertexShader = new BasicVertexShader(graphics->GetDevice());
		geometryShader = new BasicGeometryShader(graphics->GetDevice());
		pixelShader = new BasicPixelShader(graphics->GetDevice());
		// 入力レイアウトを作成
		inputLayout = new InputLayout(graphics->GetDevice(),
			VertexPositionNormal::inputElementDescs, _countof(VertexPositionNormal::inputElementDescs),
			vertexShader->GetBytecode(), vertexShader->GetBytecodeLength());
	}
	catch (const bad_alloc& err) {
		OutputDebugStringA(err.what());
		return -1;
	}
	catch (const _com_error& err) {
		OutputDebugString(err.ErrorMessage());
		return -1;
	}

	float fovAngleY = 60.0f;
	float aspectRatio = window->GetWidth() / static_cast<float>(window->GetHeight());
	float nearZ = 0.3f;
	float farZ = 1000.0f;

	timeBeginPeriod(0);

	DWORD time = timeGetTime();
	float timer = 0;

	MSG msg = {};
	while (true) {
		//時間計測
		const auto currentTime = timeGetTime();
		float deltaTime = 0;
		if (time > 0) {
			deltaTime = (currentTime - time) / 1000.0f;
		}
		time = currentTime;
		timer += deltaTime;

		// 定数バッファーへ転送するデータソースを準備
		XMMATRIX worldMatrix = XMMatrixIdentity();
		worldMatrix *= XMMatrixScaling(1.0f, 1.0f, 1.0f);
		XMVECTOR axis = XMVectorSet(1, 1, 1, 0);
		worldMatrix *= XMMatrixRotationAxis(axis, timer);
		worldMatrix *= XMMatrixTranslation(0.0f, 0.0f, 0.0f);
		// ビュー行列を計算
		XMVECTOR eyePosition = XMVectorSet(0, 1, -5, 1);
		XMVECTOR focusPosition = XMVectorSet(0, 0, 0, 1);
		XMVECTOR upDirection = XMVectorSet(0, 1, 0, 0);
		XMMATRIX viewMatrix =
			XMMatrixLookAtLH(eyePosition, focusPosition, upDirection);

		XMMATRIX projectionMatrix =
			XMMatrixPerspectiveFovLH(XMConvertToRadians(fovAngleY), aspectRatio, nearZ, farZ);

		MatricesPerFrame matricesPerFrame = {};
		XMStoreFloat4x4(
			&matricesPerFrame.worldMatrix,
			XMMatrixTranspose(worldMatrix));
		XMStoreFloat4x4(
			&matricesPerFrame.viewMatrix,
			XMMatrixTranspose(viewMatrix));
		XMStoreFloat4x4(
			&matricesPerFrame.projectionMatrix,
			XMMatrixTranspose(projectionMatrix));
		XMStoreFloat4x4(
			&matricesPerFrame.worldViewProjectionMatrix,
			XMMatrixTranspose(worldMatrix* viewMatrix* projectionMatrix));

		matricesPerFrame.lightPosition = DirectX::XMFLOAT4(1.0f, 1.0f, -2.0f, 0.0f);

		if (GetAsyncKeyState('S')) {
			matricesPerFrame.lightPosition = DirectX::XMFLOAT4(1.0f, 1.0f, -2.0f, 1.0f);
		}

		if (GetAsyncKeyState('A')) {
			matricesPerFrame.materialDiffuse = DirectX::XMFLOAT4(1, 1, 0, 1);
		}
		else {
			matricesPerFrame.materialDiffuse = DirectX::XMFLOAT4(0, 0, 0, 1);
		}
		if (GetAsyncKeyState('D')) {
			matricesPerFrame.materialSpecularColor = DirectX::XMFLOAT3(1, 1, 1);
			matricesPerFrame.materialSpecularPower = 1;
		}
		else {
			matricesPerFrame.materialSpecularColor = DirectX::XMFLOAT3(0, 0, 0);
		}

		constantBuffer->SetData(&matricesPerFrame);


		ID3D11RenderTargetView* renderTargetViews[] = { swapChain->GetRenderTargetView()};
		graphics->GetDeviceContext()->OMSetRenderTargets(1, renderTargetViews, swapChain->GetDepthStencilView());
		graphics->GetDeviceContext()->ClearRenderTargetView(renderTargetViews[0], clearColor);
		graphics->GetDeviceContext()->ClearDepthStencilView(swapChain->GetDepthStencilView(),
			D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);


		D3D11_VIEWPORT viewports[1] = { viewport };
		graphics->GetDeviceContext()->RSSetViewports(1, viewports);

		ID3D11Buffer* vertexBuffers[1] = { vertexBuffer->GetNativePointer()};
		UINT strides[1] = { sizeof(VertexPositionNormal) };
		UINT offsets[1] = { 0 };
		graphics->GetDeviceContext()->IASetVertexBuffers(
			0,
			ARRAYSIZE(vertexBuffers),
			vertexBuffers, strides, offsets);

		graphics->GetDeviceContext()->IASetIndexBuffer(indexBuffer->GetNativePointer(), DXGI_FORMAT_R32_UINT, 0);

		graphics->GetDeviceContext()->VSSetShader(vertexShader->GetNativePointer(), NULL, 0);
		graphics->GetDeviceContext()->GSSetShader(geometryShader->GetNativePointer(), NULL, 0);
		graphics->GetDeviceContext()->PSSetShader(pixelShader->GetNativePointer(), NULL, 0);


		ID3D11Buffer* constantBuffers[] = { constantBuffer->GetNativePointer()};
		graphics->GetDeviceContext()->VSSetConstantBuffers(0, _countof(constantBuffers), constantBuffers);
		graphics->GetDeviceContext()->GSSetConstantBuffers(0, _countof(constantBuffers), constantBuffers);
		graphics->GetDeviceContext()->PSSetConstantBuffers(0, _countof(constantBuffers), constantBuffers);

		graphics->GetDeviceContext()->IASetInputLayout(inputLayout->GetNativePointer());
		graphics->GetDeviceContext()->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

		graphics->GetDeviceContext()->IASetIndexBuffer(indexBuffer->GetNativePointer(), DXGI_FORMAT_R32_UINT, 0);

		// 描画
		graphics->GetDeviceContext()->DrawIndexed(indexCount, 0, 0);

		HRESULT hr = S_OK;
		swapChain->Present(1);
		if (FAILED(hr))
		{
			MessageBox(window->GetHandle(),
				L"グラフィックデバイスが物理的に取り外されたか、ドライバーがアップデートされました。",
				L"エラー", MB_OK);
			return -1;
		}

		if (PeekMessage(&msg, NULL, 0, 0, PM_NOREMOVE)) {
			if (!GetMessage(&msg, NULL, 0, 0)) {
				break;
			}
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
	timeEndPeriod(0);


	// 解放処理
	delete vertexBuffer;
	delete indexBuffer;
	delete constantBuffer;
	delete vertexShader;
	delete geometryShader;
	delete pixelShader;
	delete inputLayout;
	
	return (int)msg.wParam;
}