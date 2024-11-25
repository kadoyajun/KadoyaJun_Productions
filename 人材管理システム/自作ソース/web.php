<?php

use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider and all of them will
| be assigned to the "web" middleware group. Make something great!
|
*/

Route::get('/', function () {
    return view('welcome');
});

use App\Http\Controllers\AdminController;
Route::get('/admin',[AdminController::class,'login'])->name('login');
Route::post('/admin/login_check',[AdminController::class,'login_check']);
Route::group(['middleware' => 'auth'],function(){
    Route::get('/admin/menu',[AdminController::class,'menu']);
    Route::get('/admin/logout',[AdminController::class,'logout']);
});


use App\Http\Controllers\WorkerController;
Route::group(['middleware' => 'auth'],function(){
    Route::resource('/admin/worker',WorkerController::class);
});
use App\Http\Controllers\EventController;
Route::group(['middleware' => 'auth'],function(){
    Route::resource('/admin/event',EventController::class);
});
use App\Http\Controllers\DispatchController;
Route::group(['middleware' => 'auth'],function(){
    Route::resource('/admin/dispatch',DispatchController::class);
});