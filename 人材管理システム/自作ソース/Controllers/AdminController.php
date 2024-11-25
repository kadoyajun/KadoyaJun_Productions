<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Hash;
use Validator;
use Auth;

class AdminController extends Controller
{
    public function login(Request $request){
        // $h = Hash::make("gorin");
        // printf($h);
        // exit();
        return view('admin.index');
    }
    public function login_check(Request $request){
        $validator = Validator::make($request->all(),
            [
                'email'=>'required',
                'password' => 'required'
            ]
        );
        if($validator->fails()){
            return back()->withErrors(['メールアドレスまたはパスワードが正しくありません']);
        }
        $validated = $validator->validated();
        if(Auth::attempt($validated)){
            return redirect('/admin/menu');
        }
        return back()->withErrors(['メールアドレスまたはパスワードが正しくありません']);
    }
    public function menu(){
        return view('admin.menu');
    }
    public function logout(){
        Auth::logout();
        return redirect('/admin');
    }
}
