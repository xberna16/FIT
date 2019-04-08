from django.urls import path, include
from django.conf.urls import url

from . import views

urlpatterns = [
    path('', views.index, name='index'),
    path('mydevices/<int:device_id>/', views.repair_history, name='myDevices'),
    path('mydevices/<int:device_id>/<int:repair_id>/', views.repair_show, name='myDevices'),
    path('repair_list/<int:repair_id>/', views.repair_detail, name='repair_detail'),
    url(r'^repair/$', views.repair, name='repair'),
    url(r'^repair_list/$', views.repair_list, name='repair_list'),
    url(r'^register/$', views.register, name='register'),
    url(r'^mydevices/$',views.myDevices, name='mydevices'),
    url(r'^noDeviceUser/$',views.noDeviceUser, name='nodeviceuser'),
    url(r'^noDeviceAdmin/$',views.noDeviceAdmin, name='nodeviceadmin'),
    url(r'^verifyUser/$',views.verifyUser, name='verifyUser'),
    url(r'^newDevice/$',views.newDevice, name='newDevice'),
    url(r'^makeManager/$',views.makeManager, name='makeManager'),
]



urlpatterns += [
    path('accounts/', include('django.contrib.auth.urls')),
]