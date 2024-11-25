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
<a href="/admin/menu" class="back">戻る</a>
<h1>人材情報</h1>
<a href="/admin/worker/create">人材情報新規登録</a>
<table border="1" class="table">
	<tr>
		<th>氏名</th>
		<th>メールアドレス</th>
		<th></th>
		<th></th>
	</tr>
	@foreach($workers as $worker)
	<tr>
		<td>{{$worker->name}}</td>
		<td>{{$worker->email}}</td>
		<td><a href="/admin/worker/{{$worker->id}}/edit">編集</a></td>
		<td>
			<form method="post" action="/admin/worker/{{$worker->id}}" >
				@csrf
				@method('delete')
				<input type="submit" value="削除">
			</form>			
		</td>

	</tr>
	@endforeach
</table>
</div>
</body>
</html>