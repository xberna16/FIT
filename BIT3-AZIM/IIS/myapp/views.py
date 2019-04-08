from django.shortcuts import render, get_object_or_404, redirect
from django.http import HttpResponse
from .models import *
from django.contrib.auth.models import User, Group
from .forms import *

def makeManager(request):
    if request.method == "POST":
        form = MakeManagerForm(request.POST)
        if form.is_valid():
            cd = form.cleaned_data
            #Add user to group Manager
            Group.objects.get(name='Manager').user_set.add(cd.get('person').user)

    form = MakeManagerForm()
    return render(request, 'myapp/nodeviceadm.html', {'form': form, 'title': "Udělat z uživatele správce",})

def newDevice(request):
    # Pridavani noveho zarizeni pro admina.
    if request.method == "POST":
        form = NewDeviceForm(request.POST)
        if form.is_valid():
            post = form.save(commit=False)
            post.save()
    form = NewDeviceForm()
    return render(request, 'myapp/nodeviceadm.html', {'form': form, 'title': "Přidat nové zařízení",})

def get_user(request):
    try:
        return True, User.objects.get(username=request.user)
    except User.DoesNotExist:
        return False, redirect('/accounts/login/')

def verifyUser(request):
    if request.method == "POST":
        form = VerifyUserForm(request.POST)
        if form.is_valid():
            cd = form.cleaned_data
            #Add user to group Verified
            Group.objects.get(name='Verified').user_set.add(cd.get('person').user)

    form = VerifyUserForm()
    return render(request, 'myapp/nodeviceadm.html', {'form': form, 'title': "Ověřit uživatele",})
    

def index(request):
    # List of devices in database.
    devices_list = Device.objects.order_by("id")
    # Mapping variables into template.
    context = {
        'devices_list' : devices_list
    }
    # Rendering web page.
    return render(request, "myapp/index.html", context)

def repair_detail(request,repair_id):
    # Overeni pristupu.
    verified = False
    repair = Repair.objects.filter(id=repair_id).first()
    status, usr = get_user(request)
    if not status : return usr
    if repair.administrated_by == usr.person:
        verified = True
    # Ukladani ziskanych dat.
    if request.method == "POST":
        # Hledani pozadovaneho formulare.
        form = RepairEditForm(request.POST,user=usr)
        if form.is_valid():
            post = form.save(commit=False)
            repair.finished = post.finished
            repair.save()
            return redirect('/repair_list/')
    # Prvni zobrazeni stranky.
    form = RepairEditForm(user=usr)
    return render(request, "myapp/repair_detail.html", {'form':form,'verified':verified, 'repair':repair})


def myDevices(request):
    # Overeni prihlaseneho uzivatele.
    status, usr = get_user(request)
    if not status : return usr
    devices_list = Device.objects.filter(dused=usr.person)
    context = {
        'devices_list' : devices_list
    }
    return render(request, 'myapp/list_devices.html',context)

def repair_show(request,device_id,repair_id):
    show = False
    # Prihlaseny uzivatel.
    status, usr = get_user(request)
    if not status : return usr
    # Zaznam o oprave.
    repair = Repair.objects.filter(id=repair_id).first()
    if repair == None:
        return redirect('/mydevices/')
    # Zarizeni patri prihlasenemu uzivateli.
    if DevicePersonUsed.objects.filter(device=repair.device,person=usr.person) != None:
        show = True
    # Overeni spravce zarizeni.
    if DevicePersonAdmin.objects.filter(device=repair.device,person=usr.person) != None:
        show = True
    return render(request, 'myapp/repair_show.html',{'show':show,'repair':repair})

def repair_history(request,device_id):
    show = True
    # Prihlaseny uzivatel.
    status, usr = get_user(request)
    if not status : return usr
    # Zarizeni patri prihlasenemu uzivateli.
    if DevicePersonUsed.objects.filter(device=device_id,person=usr.person) == None:
        show = False
    # Je spravcem uzivatele.
    if Device.objects.filter(id=device_id,dadmin=usr.person) == None:
        show = False
    repairs_list = Repair.objects.filter(device=device_id)
    return render(request, 'myapp/repair_history.html',{'repairs_list':repairs_list,'show':show})

def repair_list(request):
    status, usr = get_user(request)
    if not status : return usr
    repairs_list = Repair.objects.filter(administrated_by=usr.person).exclude(finished__isnull=True)
    repairs_tbdone= Repair.objects.filter(administrated_by=usr.person).filter(finished__isnull=True)
    return render(request, 'myapp/repair_list.html',{'repairs_list':repairs_list, 'repairs_tbdone': repairs_tbdone})

def repair(request):
    admin = True
    # Ukladani ziskanych dat.
    if request.method == "POST":
        form = RepairForm(request.POST,user=request.user)
        if form.is_valid():
            post = form.save(commit=False)
            status, usr = get_user(request)
            if not status : return usr
            post.reported_by = usr.person
            if post.device.dadmin.first() == None:
                admin = False
            else:
                post.administrated_by = post.device.dadmin.first()
                post.finished = None
                post.save()
    # Prvni zobrazeni stranky.
    form = RepairForm(user=request.user)
    return render(request, 'myapp/repair.html', {'form': form,'admin':admin})

def noDeviceUser(request):
    if request.method == "POST":
        form = NoDeviceUserForm(request.POST)
        if form.is_valid():
            post = form.save(commit=False)
            post.save()
    form = NoDeviceUserForm()
    return render(request, 'myapp/nodeviceadm.html', {'form': form, 'title': "Přidělit zařízení uživateli",})

def noDeviceAdmin(request):
    if request.method == "POST":
        form = NoDeviceAdminForm(request.POST)
        if form.is_valid():
            post = form.save(commit=False)
            post.save()
    form = NoDeviceAdminForm()
    return render(request, 'myapp/nodeviceadm.html', {'form': form, 'title': "Přidělit zařízení správci",})

def register(request):
    # Ukladani ziskanych dat.
    if request.method == "POST":
        form = RegisterForm(request.POST)
        if form.is_valid():
            post = form.save(commit=False)
            passwd = request.POST.get('passwd')
            post.user = User.objects.create_user(post.pid, post.mail, passwd)
            post.save()
    # Prvni zobrazeni stranky.
    form = RegisterForm()
    
    return render(request, 'myapp/register.html', {'form': form,})
