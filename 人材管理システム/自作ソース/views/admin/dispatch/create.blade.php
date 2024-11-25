<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/style.css">
</head>
<body>
<div class="wrap">
<a href="/admin/dispatch" class="back">戻る</a>
<h1>派遣情報新規登録</h1>
<form method="post" action="/admin/dispatch">
    @csrf
    <div class="input">
    <select name="event_id" id="">
        <option value="" hidden>イベント名</option>
        @foreach($events as $event)
            <option value="{{$event->id}}">{{$event->title}}</option>
        @endforeach
    </select>
    <select name="worker_id" id="">
        <option value="" hidden>人材名</option>
        @foreach($workers as $worker)
            <option value="{{$worker->id}}">{{$worker->name}}</option>
        @endforeach
    </select>
    <input type="submit" value="作成"></input>
    </div>
</form>
</div>
</body>
</html>