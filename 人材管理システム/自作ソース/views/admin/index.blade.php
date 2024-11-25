<!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<link rel="stylesheet" type="text/css" href="style.css">
	<title></title>
</head>
<body>
<div class="wrap">
	<h1>管理者ログイン</h1>
	<form method="post" action="/admin/login_check">
		@csrf
		ID:<input type="text" name="email"><br>
		PASS<input type="password" name="password"><br>
		<input type="submit" value="ログイン">
	</form>
</div>
</body>
</html>