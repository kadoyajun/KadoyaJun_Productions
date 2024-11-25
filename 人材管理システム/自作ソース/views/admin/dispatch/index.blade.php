<!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/style.css">
</head>
<body>
<?php
    $eventId = array(null);
    foreach($dispatches as $dispatch){
        foreach($events as $event){
            if($dispatch->event_id == $event->id){
                array_push($eventId,$event->title);
                break;
            }
        }
    }
    $workerId = array(null);
    foreach($dispatches as $dispatch){
        foreach($workers as $worker){
            if($dispatch->worker_id == $worker->id){
                array_push($workerId, $worker->name);
                break;
            }
        }
    }
	$count = 1;
?>
<div class="wrap">
<a href="/admin/menu" class="back">戻る</a>
<h1>派遣情報</h1>
<a href="/admin/dispatch/create">派遣情報新規登録</a>
<table border="1" class="table">
	<tr>
		<th>イベント名</th>
		<th>人材名</th>
		<th></th>
		<th></th>
	</tr>
	@foreach($dispatches as $dispatch)
	<tr>
        <td>{{$eventId[$count]}}</td>
        <td>{{$workerId[$count]}}</td>
		<td><a href="/admin/dispatch/{{$dispatch->id}}/edit">編集</a></td>
		<td>
			<form method="post" action="/admin/dispatch/{{$dispatch->id}}">
				@csrf
				@method('delete')
				<input type="submit" value="削除">
			</form>			
		</td>

	</tr>
	<?php $count++; ?>
	@endforeach
</table>
</div>
</body>
</html>