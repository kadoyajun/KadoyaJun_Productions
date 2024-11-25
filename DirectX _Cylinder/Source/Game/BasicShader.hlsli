cbuffer ConstantBuffer
{
    matrix World; //���[���h�ϊ��s��
    matrix View; //�r���[�ϊ��s��
    matrix Projection; //�����ˉe�ϊ��s��
    matrix WorldViewProjection; // WVP�s��
    
	// �J�����̈ʒu���W
    float4 ViewPosition;
	// ���C�g�̈ʒu���W(���s���� w = 0, �_���� w = 1)
    float4 LightPosition;
	// �}�e���A���̕\�ʃJ���[
    float4 MaterialDiffuse;

	// �}�e���A���̋��ʔ��˃J���[
    float3 MaterialSpecularColor;
	// �}�e���A���̋��ʔ��˂̋���
    float MaterialSpecularPower;
}

// ���_�V�F�[�_�[�̓��̓f�[�^
struct VSInput
{
    float4 position : POSITION;
    float3 normal : NORMAL;
};

// ���_�V�F�[�_�[�̏o�̓f�[�^
struct VSOutput
{
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
};

// �W�I���g���[�V�F�[�_�[�̓��̓f�[�^
typedef VSOutput GSInput;

// �W�I���g���[�V�F�[�_�[�̏o�̓f�[�^
struct GSOutput
{
    float4 position : SV_POSITION;
    float4 worldPosition : POSITION;
    float3 worldNormal : NORMAL;
};

// �s�N�Z���V�F�[�_�[�̓��̓f�[�^
typedef GSOutput PSInput;
