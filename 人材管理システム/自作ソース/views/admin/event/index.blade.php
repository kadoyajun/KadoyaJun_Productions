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
<h1>イベント情報</h1>
<a href="/admin/event/create">イベント新規登録</a>
<table border="1" class="table">
	<tr>
		<th>イベント名</th>
		<th>開催場所</th>
		<th>開催日時</th>
		<th></th>
		<th></th>
	</tr>
	@foreach($events as $event)
	<tr>
		<td>{{$event->title}}</td>
		<td>{{$event->place}}</td>
		<td>{{$event->event_date}}</td>
		<td><a href="/admin/event/{{$event->id}}/edit">編集</a></td>
		<td>
			<form method="post" action="/admin/event/{{$event->id}}">
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