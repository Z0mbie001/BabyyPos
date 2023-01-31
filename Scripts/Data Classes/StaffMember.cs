using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffMember
{
    public int staffID;
    public string lastName;
    public string firstName;
    public DateTime dateOfBirth;
    public DateTime startDate;
    public DateTime endDate;
    public int permissionLevel;

    //Initilising procedure
    public StaffMember(int idToSet, string lastNameToSet, string firstNameToSet, DateTime dateOfBirthToSet, DateTime startDateToSet, DateTime endDateToSet, int permissionLevelToSet)
    {
        staffID = idToSet;
        lastName = lastNameToSet;
        firstName = firstNameToSet;
        dateOfBirth = dateOfBirthToSet;
        startDate = startDateToSet;
        endDate = endDateToSet;
        permissionLevel = permissionLevelToSet;
    }
}
