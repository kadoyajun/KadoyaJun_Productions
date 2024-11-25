<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Dispatch;
use App\Models\Event;
use App\Models\Worker;
use Validator;

class DispatchController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $dispatches = Dispatch::all();
        $events = Event::all();
        $workers = Worker::all();
        return view('admin.dispatch.index',['dispatches' => $dispatches,'events' => $events, 'workers'=>$workers]);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        $events = Event::all();
        $workers = Worker::all();
        return view('admin.dispatch.create',['events' => $events, 'workers'=>$workers]);
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validator = Validator::make($request->all(),
            [
                'event_id'=>'required',
                'worker_id'=>'required'
            ]
        );
        if($validator->fails()){
            return back();
        }
        $validated = $validator->validated();
        $dispatch = new Dispatch;
        $dispatch->event_id = $validated['event_id'];
        $dispatch->worker_id = $validated['worker_id'];
        $dispatch->save();
        return redirect('/admin/dispatch');
    }

    /**
     * Display the specified resource.
     */
    public function show(string $id)
    {
        //
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(string $id)
    {
        $dispatch = Dispatch::find($id);
        $events = Event::all();
        $workers = Worker::all();
        return view('admin.dispatch.edit',['dispatch' => $dispatch,'events' => $events, 'workers'=>$workers]);
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, string $id)
    {
        $validator = Validator::make($request->all(),
            [
                'event_id'=>'required',
                'worker_id'=>'required'
            ]
        );
        if($validator->fails()){
            return back();
        }
        $validated = $validator->validated();
        $dispatch = Dispatch::find($id);
        $dispatch->event_id = $validated['event_id'];
        $dispatch->worker_id = $validated['worker_id'];
        $dispatch->save();
        return redirect('/admin/dispatch');
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(string $id)
    {
        $dispatch = Dispatch::destroy($id);
        return redirect('/admin/dispatch');
    }
}
