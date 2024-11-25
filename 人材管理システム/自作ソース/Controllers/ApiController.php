<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Event;
use App\Models\Dispatch;

class ApiController extends Controller
{
    public function getEventData(Request $request){
        if(!($request->worker_id)){
            return response()->json(['worker_idがありません'],400);
        }
        if($request->date){
            if($request->place){
                if($request->title){
                    $event = Event::where('event_date',$request->date)->where('place',$request->place)->where('title',$request->title)->first();
                    $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$event->id)->get();
                }
                else{
                    $event = Event::where('event_date',$request->date)->where('place',$request->place)->first();
                    $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$event->id)->get();
                }
            }else{
                if($request->title){
                    $event = Event::where('event_date',$request->date)->where('title',$request->title)->first();
                    $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$event->id)->get();
                }
                else{
                    $event = Event::where('event_date',$request->date)->first();
                    $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$event->id)->get();
                }
            }
        }
        else{
            if($request->place){
                if($request->title){
                    $event = Event::where('place',$request->place)->where('title',$request->title)->first();
                    $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$event->id)->get();
                }
                else{
                    $event = Event::where('place',$request->place)->first();
                    $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$event->id)->get();
                }
            }else{
                if($request->title){
                    $event = Event::where('title',$request->title)->first();
                    $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$event->id)->get();
                }
                else{
                    $data = Dispatch::where('worker_id',$request->worker_id)->get();
                }
            }
        }
        if(!empty($data[0]->id)){
            return response()->json($data,200);
        }
        return response()->json(['エラー'],404);
    }
    public function acceptEventData(Request $request){
        if(!($request->worker_id||$request->event_id)){
            return response()->json(['エラー'],404);
        }
        $data = Dispatch::where('worker_id',$request->worker_id)->where('event_id',$request->event_id)->get();
        if(empty($data[0]->id)){
            return response()->json('エラー',404);
        }
        for($i = 0;$i < count($data); $i++){
            $data[$i]->accept_flag = true;
            $data[$i]->save();
        }
        return response()->json('成功',200);
    }
}
