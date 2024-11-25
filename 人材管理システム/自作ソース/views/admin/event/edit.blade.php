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
<a href="/admin/event" class="back">戻る</a>
<h1>イベント情報更新</h1>
<form method="post" action="/admin/event/{{$event->id}}">
	@csrf
	@method('put')
	<div class="input">
		<div>イベント名：<input type="text" name="title" value="{{$event->title}}"></div>
		<div>開催場所：<input type="text" name="place" value="{{$event->place}}"></div>
		<div>開催日時：<input type="date" name="event_date" value="{{$event->event_date}}"></div>
		<div class="submit"><input type="submit" value="保存"></div>
	</div>
</form>
</div>
</body>
</html>