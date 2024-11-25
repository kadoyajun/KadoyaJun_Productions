<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Worker;
use Validator;
use Hash;

class WorkerController extends Controller
{
    public function index()
    {
        $workers = Worker::all();
        return view('admin.worker.index',['workers'=>$workers]);
    }

    public function create()
    {
        return view('admin.worker.create');
    }

    public function store(Request $request)
    {
        $validator = Validator::make($request->all(),
            [
                'name'=>'required',
                'email'=>'required',
                'password'=>'required',
                'memo'=>'max:255'
            ]
        );
        if($validator->fails()){
            return redirect('/admin/worker')->withErrors(["エラーが発生しました"]);
        }
        $validated = $validator->validated();
        $worker = new Worker;
        $worker->name = $validated['name'];
        $worker->email = $validated['email'];
        $worker->password = Hash::make($validated['password']);
        $worker->memo = $validated['memo'];
        $worker->save();
        return redirect('/admin/worker');
    }

    public function edit(string $id)
    {
        $worker = Worker::find($id);
        return view('admin.worker.edit',['worker'=>$worker]);
    }
    
    public function update(Request $request, string $id)
    {
        $validator = Validator::make($request->all(),
            [
                'name'=>'required',
                'email'=>'required',
                'password'=>'max:255',
                'memo'=>'max:255'
            ]
        );
        if($validator->fails()){
            return back()->withErrors('エラーが発生しました');
        }
        $validated = $validator->validated();
        $worker = Worker::find($id);
        $worker->name = $validated['name'];
        $worker->email = $validated['email'];
        if($validated['password']){
            $worker->password = $validated['password'];
        }
        $worker->memo = $validated['memo'];
        $worker->save();
        return redirect('/admin/worker');
    }

    public function destroy(string $id)
    {
        $worker = Worker::destroy($id);
        return redirect('/admin/worker');
    }
}
