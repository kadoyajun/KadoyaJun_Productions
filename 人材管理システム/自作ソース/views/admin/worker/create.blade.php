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
<h1>人材の新規登録</h1>
<form method="post" action="/admin/worker">
	@csrf
	<div class="input">
		<div>氏名<input type="text" name="name"></div>
		<div>メールアドレス<input type="text" name="email"></div>
		<div>パスワード<input type="password" name="password"></div>
		<div>メモ<input type="text" name="memo"></div>
		<div class="submit"><input type="submit" value="登録"></div>
	</div>
</form>
</div>
</body>
</html>