SELECT People.PersonId,People.Name,Phones.PhoneId,Phones.Number,Phones.Person_PersonId FROM People LEFT OUTER JOIN Phones ON People.PersonId = Phones.PhoneId
