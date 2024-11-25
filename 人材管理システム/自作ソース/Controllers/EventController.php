<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Event;
use Hash;
use Validator;

class EventController extends Controller
{
    public function index()
    {
        $events = Event::all();
        return view('admin.event.index',['events'=>$events]);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('admin.event.create');
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validator = Validator::make($request->all(),
            [
                'title'=>'required',
                'place'=>'required',
                'event_date'=>'required'
            ]
        );
        if($validator->fails()){
            return back();
        }
        $validated = $validator->validated();
        $event = new Event;
        $event->title = $validated['title'];
        $event->place = $validated['place'];
        $event->event_date = $validated['event_date'];
        $event->save();
        return redirect('/admin/event');
    }

    public function show($id){

    }
    /**
     * Show the form for editing the specified resource.
     */
    public function edit(string $id)
    {
        $event = event::find($id);
        return view('admin.event.edit',['event'=>$event]);
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, string $id)
    {
        $validator = Validator::make($request->all(),
            [
                'title'=>'required',
                'place'=>'required',
                'event_date'=>'required'
            ]
        );
        if($validator->fails()){
            return back()->withErrors('エラーが発生しました');
        }
        $validated = $validator->validated();
        $event = Event::find($id);
        $event->title = $validated['title'];
        $event->place = $validated['place'];
        $event->event_date = $validated['event_date'];
        $event->save();
        return redirect('/admin/event');
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(string $id)
    {
        $event = Event::destroy($id);
        return redirect('/admin/event');
    }
}
