import time
import datetime
from django import forms
from .models import *
from django.contrib.auth.models import User

class MakeManagerForm(forms.Form):
    person = forms.ModelChoiceField(queryset= Person.objects.filter(user__in=User.objects.filter(groups__name='Verified')).exclude(user__in=User.objects.filter(groups__name='Manager')))
    def __init__(self, *args, **kwargs):
        super(MakeManagerForm, self).__init__(*args, **kwargs)
        self.fields["person"].label = "Uživatel"

class NewDeviceForm(forms.ModelForm):
    buydate = forms.DateTimeField(input_formats=["%d.%m.%Y",])
    def __init__(self, *args, **kwargs):
        super(NewDeviceForm, self).__init__(*args, **kwargs)
        new_date = datetime.datetime.now().strftime('%d.%m.%Y')
        self.fields["buydate"].initial = new_date
        self.fields["dused"].queryset = Person.objects.filter(user__in=User.objects.filter(groups__name='Verified'))
        self.fields["dadmin"].queryset = Person.objects.filter(user__in=User.objects.filter(groups__name='manager'))
        self.fields["buydate"].label = "Datum zakoupení"

    class Meta:
        model = Device
        fields=('buydate', 'serialnumber','manufacturer','room','department','type','dused','dadmin')
        labels = {
            "serialnumber":"Sériové číslo",
            "manufacturer":"Výrobce",
            "room":"Místnost",
            "department":"Oddělení",
            "type":"Typ zařízení",
            "dused":"Uživatelé",
            "dadmin":"Správci",
        }
class VerifyUserForm(forms.Form):
    person = forms.ModelChoiceField(queryset= Person.objects.exclude(user__in=User.objects.filter(groups__name='Verified')))
    def __init__(self, *args, **kwargs):
        super(VerifyUserForm, self).__init__(*args, **kwargs)
        self.fields["person"].label = "Uživatel"

class NoDeviceUserForm(forms.ModelForm):
    def __init__(self, *args, **kwargs):
        super(NoDeviceUserForm, self).__init__(*args, **kwargs)
        self.fields["person"].queryset = Person.objects.filter(user__in=User.objects.filter(groups__name='Verified'))
        self.fields["device"].queryset = queryset=Device.objects.filter(dused=None)
    
    class Meta:
        model = DevicePersonUsed
        fields=('device', 'person')
        labels = {
            "device": "Zařízení",
            "person": "Uživatel",
        }
        
class NoDeviceAdminForm(forms.ModelForm):
    def __init__(self, *args, **kwargs):
        super(NoDeviceAdminForm, self).__init__(*args, **kwargs)
        self.fields["person"].queryset = Person.objects.filter(user__in=User.objects.filter(groups__name='manager'))
        self.fields["device"].queryset = queryset = Device.objects.filter(dadmin=None)
    
    class Meta:
        model = DevicePersonAdmin
        fields=('device', 'person')
        labels = {
            "device": "Zařízení",
            "person": "Správce",
        }



class RepairEditForm(forms.ModelForm):
    finished = forms.DateTimeField(input_formats=["%d.%m.%Y %H:%M:%S","",])
    def __init__(self, *args, **kwargs):
        self.user = kwargs.pop('user',None)
        super(RepairEditForm, self).__init__(*args, **kwargs)
        new_date = datetime.datetime.now().strftime('%d.%m.%Y %H:%M:%S')
        self.fields["finished"].initial = new_date
        self.fields["administrated_by"].initial = self.user.person
        self.fields["finished"].label = "Čas dokončení"
        self.fields["finished"].required= False

    class Meta:
        model = Repair
        fields = ('finished', 'administrated_by')
        exclude = ['reported','device','description']
        labels = {
            "administrated_by": "Správce",
        }

class RepairForm(forms.ModelForm):
    
    reported = forms.DateTimeField(input_formats=["%d.%m.%Y %H:%M:%S",])
    
    def __init__(self, *args, **kwargs):
        self.user = kwargs.pop('user',None)
        super(RepairForm, self).__init__(*args, **kwargs)
        new_date = datetime.datetime.now().strftime("%d.%m.%Y %H:%M:%S")
        self.fields["reported"].initial = new_date
         # Neprihlaseny uzivatel.
        try:
            self.fields['device'].queryset = Device.objects.filter(dused=self.user.person)
        except AttributeError:
            self.fields['device'].queryset = None
        self.fields['reported'].label = "Čas nahlášení"

    class Meta:
        model = Repair
        fields = ('reported',  'device', 'description')
        labels = {
            "device": "Zařízení",
            "description": "Popis problému",
        }

class RegisterForm(forms.ModelForm):
    passwd = forms.CharField(widget=forms.PasswordInput)
    widgets={
            'passwd' : forms.PasswordInput(),
    }
    def __init__(self,*args, **kwargs):
        super(RegisterForm, self).__init__(*args, **kwargs)
        self.fields['passwd'].label = "Heslo"
    class Meta:
        model = Person
        labels = {
            "ddepartment": "Oddělení",
            "name": "Jméno",
            "surname": "Příjmení",
            "pid": "Identifikace",
            "room": "Místnost",
            "mail": "E-mail",
        }
        fields = ('name', 'surname', 'pid', 'room','mail','ddepartment')
