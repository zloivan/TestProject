using System.Collections;
using System.Collections.Generic;
using Services.Input;
using UnityEngine;
using Zenject;

public class ServiceInstaller : MonoInstaller
{
   public override void InstallBindings()
   {
      InputServiceInstaller.Install(Container);
   }
}
