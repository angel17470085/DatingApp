import { Photo } from "./photo";


export interface Member {
    id:           number;
    userName:     string;
    photoUrl:     string;
    age:          number;
    knownAs:      string;
    created:      Date;
    lastActive:   Date;
    gender:       string;
    introduction: string;
    lookingFor:   string;
    interests:     null;
    city:         string;
    country:      string;
    photos:       Photo[];
}


