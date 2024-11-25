<!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/style.css">
</head>
<body>
<div class="wrap">
<a href="/admin/worker" class="back">戻る</a>
<h1>人材情報更新</h1>
<form method="post" action="/admin/worker/{{$worker->id}}">
	@csrf
	@method('put')
	<div class="input">
		<div>氏名<input type="text" name="name" value="{{$worker->name}}"></div>
		<div>メールアドレス<input type="text" name="email" value="{{$worker->email}}"></div>
		<div>パスワード<input type="password" name="password"></div>
		<div>メモ<input type="text" name="memo" value="{{$worker->memo}}"></div>
		<div class="submit"><input type="submit" value="更新"></div>
	</div>
</form>
</div>
</body>
</html>