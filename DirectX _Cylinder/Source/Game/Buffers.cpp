#include "Game.h"
#include <comdef.h>

// 頂点バッファー
VertexBuffer::VertexBuffer(ID3D11Device* graphicsDevice, UINT byteWidth){

	D3D11_BUFFER_DESC bufferDesc = {};
	bufferDesc.ByteWidth = byteWidth;
	bufferDesc.Usage = D3D11_USAGE_DEFAULT;
	bufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
	bufferDesc.CPUAccessFlags = 0;
	bufferDesc.MiscFlags = 0;
	bufferDesc.StructureByteStride = 0; 
	const auto hr = graphicsDevice->CreateBuffer(&bufferDesc, nullptr, &buffer);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}
};

ID3D11Buffer* VertexBuffer::GetNativePointer()
{
	return buffer.Get();
}

void VertexBuffer::SetData(void* data) {

	ID3D11Device* graphicsDevice = nullptr;
	buffer->GetDevice(&graphicsDevice);
	ID3D11DeviceContext* immediateContext = nullptr;
	graphicsDevice->GetImmediateContext(&immediateContext);

	immediateContext->UpdateSubresource(
		buffer.Get(), 0, NULL, data, 0, 0);

	SAFE_RELEASE(immediateContext);
	SAFE_RELEASE(graphicsDevice);
}


// インデックスバッファー
IndexBuffer::IndexBuffer(ID3D11Device* graphicsDevice, UINT indexCount)
{
		D3D11_BUFFER_DESC bufferDesc = {};
		bufferDesc.ByteWidth = indexCount * sizeof(UINT32);
		bufferDesc.Usage = D3D11_USAGE_DEFAULT;
		bufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
		bufferDesc.CPUAccessFlags = 0;
		bufferDesc.MiscFlags = 0;
		bufferDesc.StructureByteStride = 0;
		const auto hr = graphicsDevice->CreateBuffer(&bufferDesc, nullptr, &buffer);
		if (FAILED(hr)) {
			OutputDebugString(L"インデックスバッファーを作成できませんでした。");
			throw _com_error(hr);
		}
};


void IndexBuffer::SetData(UINT32* data) {

	ID3D11Device* graphicsDevice = nullptr;
	buffer->GetDevice(&graphicsDevice);
	ID3D11DeviceContext* immediateContext = nullptr;
	graphicsDevice->GetImmediateContext(&immediateContext);

	immediateContext->UpdateSubresource(
		buffer.Get(), 0, NULL, data, 0, 0);

	SAFE_RELEASE(immediateContext);
	SAFE_RELEASE(graphicsDevice);
}

ID3D11Buffer* IndexBuffer::GetNativePointer()
{
	return buffer.Get();
}

// 定数バッファー
ConstantBuffer::ConstantBuffer(ID3D11Device* graphicsDevice, UINT byteWidth)
{
	D3D11_BUFFER_DESC bufferDesc = { 0 };
	bufferDesc.ByteWidth = byteWidth;
	bufferDesc.Usage = D3D11_USAGE_DEFAULT;
	bufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
	bufferDesc.CPUAccessFlags = 0;
	bufferDesc.MiscFlags = 0;
	bufferDesc.StructureByteStride = 0; 
	const auto hr = graphicsDevice->CreateBuffer(&bufferDesc, nullptr, &buffer);
	if (FAILED(hr)) {
		OutputDebugString(L"定数バッファーを作成できませんでした。");
		throw _com_error(hr);
	}
};

void ConstantBuffer::SetData(void* data) {

	ID3D11Device* graphicsDevice = nullptr;
	buffer->GetDevice(&graphicsDevice);
	ID3D11DeviceContext* immediateContext = nullptr;
	graphicsDevice->GetImmediateContext(&immediateContext);

	immediateContext->UpdateSubresource(
		buffer.Get(), 0, NULL, data, 0, 0);

	SAFE_RELEASE(immediateContext);
	SAFE_RELEASE(graphicsDevice);
}

ID3D11Buffer* ConstantBuffer::GetNativePointer() {
	return buffer.Get();
}